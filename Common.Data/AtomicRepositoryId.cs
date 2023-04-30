using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Common.Data.Models;
using Common.Data.Extensions;

namespace Common.Data;

public interface IRepository<T>
{
    Task<IList<T>> GetAll();
    Task<T?> Get(int PrimaryKey);
    Task<IList<T>> Get<T2>(Expression<Func<T, bool>> predicate, Expression<Func<T, T2>> order);
    Task<int> Save(T Item);
    Task<int> Save(IEnumerable<T> Items);
    Task<int> Delete(int PrimaryKey);
    Task<int> Delete(IEnumerable<int> PrimaryKeys);
}

/// <summary>
/// This Repository Removes Unit of Work from the Repository and forces actions to be 
/// atomic.  When there are not business reasons to do multiple actions in a single transaction
/// this allows the business layer to ignore the database layer
/// </summary>
/// <typeparam name="T"></typeparam>
public class AtomicRepositoryId<T> : IRepository<T> where T : EntityDeletionId
{
    static string _tableName = typeof(T).Name;

    protected DbContext _context;
    public AtomicRepositoryId(DbContext context)
    {
        _context = context;
    }

    private IQueryable<T> Get()
    {
        return _context.Set<T>().AsNoTracking().Where(e => !e.IsDeleted);
    }

    public virtual async Task<T?> Get(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IList<T>> GetAll()
    {
        return await Get().ToListAsync();
    }

    public virtual async Task<IList<T>> Get(Expression<Func<T, bool>> predicate)
    {
        return await Get().Where(predicate).ToListAsync();
    }

    public virtual async Task<IList<T>> Get<T2>(Expression<Func<T, bool>> predicate, Expression<Func<T, T2>> order)
    {
        return await Get().Where(predicate).OrderBy(order).ToListAsync();
    }

    public virtual async Task<T> GetSingleExisting(Expression<Func<T, bool>> predicate)
    {
        return await Get().Where(predicate).SingleOrDefaultAsync();
    }

    private async Task<bool> AddOrUpdate(T item)
    {
        bool isAdded = false;
        if (item != null)
        {
            var existing = await _context.Set<T>()
                .Where(i => i.Id == item.Id).FirstOrDefaultAsync();
            await _context.UpdateOrAddAsync(item, existing);
            isAdded = existing is null;
        }
        return isAdded;
    }

    private async Task<bool> AddOrUpdate(T item, Expression<Func<T, bool>> predicate)
    {
        bool isAdded = false;
        if (item != null)
        {
            var count = await _context.UpdateOrAddAsync(item, predicate);
            isAdded = count > 0;
        }
        return isAdded;
    }

    private async Task<int> AddOrUpdateRange(IEnumerable<T> items)
    {
        int added = 0;
        if (items != null)
            foreach (T item in items)
                if(await AddOrUpdate(item))
                    added++;
        return added;
    }

    //private async Task<int> AddOrUpdateRange(IEnumerable<T> items, Expression<Func<T, bool>> predicate)
    //{
    //    int added = 0;
    //    if (items != null)
    //        foreach (T item in items)
    //            if (await AddOrUpdate(item, predicate))
    //                added++;
    //    return added;
    //}

    private async Task<int> SaveTransaction(Task addOrUpdateTask)
    {
        int changeCount = 0;
        try
        {
            using var transaction = _context.Database.BeginTransaction();
            await addOrUpdateTask;
            changeCount = await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            var e = ex.InnerException ?? ex;
            System.Diagnostics.Trace.TraceError("Failed to update {0} record in the database. " +
                "Transaction has been rolled back automatically on disposal. {1}", _tableName, e);
            System.Diagnostics.Debugger.Break();
            throw;
        }
        return changeCount;
    }

    public async Task<int> AddOverwriteAsync<TDataType>(IEnumerable<TDataType> entities, CancellationToken ct = default) where TDataType : class
    {
        int updateCount = 0;
        if (!entities.IsNull() && !ct.IsCancellationRequested)
        {
            string tableName = typeof(TDataType).Name;
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                //await this.TruncateAsync<T>("dbo"); // faster but requires ALTER permissions
                await _context.DeleteAsync<TDataType>("dbo"); // slower but only requires DELETE permissions
                await _context.AddRangeAsync(entities);
                int changeCount = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                int totalCount = entities.Count();
                System.Diagnostics.Trace.TraceInformation("Added {0} of {1} records to {2} ({3} changed)",
                    updateCount, totalCount, tableName, changeCount);
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Trace.TraceWarning("Database connection disposed prematurely. {0}", ex);
            }
            catch (Exception ex)
            {
                var e = ex.InnerException ?? ex;
                System.Diagnostics.Trace.TraceError("Failed to update {0} record in the database. " +
                    "Transaction has been rolled back automatically on disposal. {1}", tableName, e);
                System.Diagnostics.Debugger.Break();
            }
        }
        return updateCount;
    }

    public virtual async Task<int> Save(T item)
    {
        var task = AddOrUpdate(item);
        return await SaveTransaction(task);
    }

    //public virtual async Task<int> Save(IEnumerable<T> items)
    //{
    //    var task = AddOrUpdateRange(items);
    //    int changeCount = await SaveTransaction(task);
    //    int updateCount = changeCount > 0 ? items.Count() : 0;
    //    return updateCount;
    //}

    //public virtual async Task<int> SaveRange(IEnumerable<T> items, Expression<Func<T, bool>> predicate)
    //{
    //    int updateCount = 0;
    //    if (items != null)
    //    {
    //        int totalItemCount = items.Count();
    //        try
    //        {
    //            using var transaction = _context.Database.BeginTransaction();
    //            int addCount = await AddOrUpdateRange(items, predicate);
    //            int changeCount = await _context.SaveChangesAsync();
    //            await transaction.CommitAsync();
    //            updateCount = totalItemCount;
    //            _logger.LogDebug("Added {0} of {1} records to {2} ({3} changed)",
    //                addCount, totalItemCount, _tableName, changeCount);
    //        }
    //        catch (Exception ex)
    //        {
    //            var e = ex.InnerException ?? ex;
    //            _logger.LogWarning(e, "Failed to update {0} record in the database. " +
    //                "Transaction has been rolled back automatically on disposal. " +
    //                "Trying again individually.", _tableName);
    //            updateCount = await Save(items);
    //        }
    //    }
    //    return updateCount;
    //}

    public virtual async Task<int> SaveRange(IEnumerable<T> items)
    {
        int updateCount = 0;
        if (items != null)
        {
            int totalItemCount = items.Count();
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                int addCount = await AddOrUpdateRange(items);
                int changeCount = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                updateCount = totalItemCount;
                System.Diagnostics.Trace.TraceInformation("Added {0} of {1} records to {2} ({3} changed)",
                    addCount, totalItemCount, _tableName, changeCount);
            }
            catch (Exception ex)
            {
                var e = ex.InnerException ?? ex;
                System.Diagnostics.Trace.TraceWarning("Failed to update {0} record in the database. " +
                    "Transaction has been rolled back automatically on disposal. " +
                    "Trying again individually. {1}", _tableName, e);
                updateCount = await Save(items);
            }
        }
        return updateCount;
    }

    public virtual async Task<int> Save(IEnumerable<T> items)
    {
        int updateCount = 0;
        if (items != null)
        {
            int totalItemCount = items.Count();
            foreach (T item in items)
            {
                try
                {
                    using var transaction = _context.Database.BeginTransaction();
                    bool isAdded = await AddOrUpdate(item);
                    int changeCount = await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    updateCount++;
                    System.Diagnostics.Trace.TraceInformation("{0} {1} of {2} records to {3} ({4} change)",
                        isAdded ? "Added" : "Updated", updateCount, totalItemCount, _tableName, changeCount);
                }
                catch (Exception ex)
                {
                    var e = ex.InnerException ?? ex;
                    System.Diagnostics.Trace.TraceError("Failed to update {0} record in the database. " +
                        "Transaction has been rolled back automatically on disposal. {1}", _tableName, e);
                    System.Diagnostics.Debugger.Break();
                    throw;
                }
            }
        }
        return updateCount;
    }

    private async Task SoftDelete(int id)
    {
        //_context.Remove(await GetByID(id));
        var item = await Get(id);
        if (item != null)
            item.IsDeleted = true;
    }

    public virtual async Task<int> Delete(int id)
    {
        await SoftDelete(id);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int> Delete(IEnumerable<int> ids)
    {
        foreach (int id in ids)
            await SoftDelete(id);
        return await _context.SaveChangesAsync();
    }
}