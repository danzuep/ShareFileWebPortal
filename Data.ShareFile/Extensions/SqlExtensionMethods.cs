using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Data.ShareFile.Models;
using System.Threading;
using Microsoft.Data.SqlClient;

namespace Data.ShareFile.Extensions
{
    internal static class SqlExtensionMethods
    {
        public static bool IsNullOrEmpty<T>( [NotNullWhen(false)] 
           this IEnumerable<T> enumerable)
           => !enumerable?.Any() ?? true;

        public static bool IsNotNullOrEmpty<T>( [NotNullWhen(true)] 
            this IEnumerable<T> enumerable)
            => enumerable != null && enumerable.Any();

        public static bool IsNullOrEmpty<T>( [NotNullWhen(false)] 
            this ICollection<T> list) => !list.IsNotNullOrEmpty();

        public static bool IsNotNullOrEmpty<T>( [NotNullWhen(true)] 
            this ICollection<T> list) => list?.Count > 0;

        internal static string ToEnumeratedString<T>(
            this IEnumerable<T> data, string div = ", ")
            => data == null ? "" : string.Join(div,
                data.Select(o => o?.ToString() ?? ""));

        internal static DbContextOptions<TContext> GetDbContextOptions<TContext>(
            string connectionString) where TContext : DbContext =>
            new DbContextOptionsBuilder<TContext>()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlServer(connectionString, o => {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    //o.EnableRetryOnFailure(retryCount); // doesn't work with transactions
                }).Options;

        internal static IQueryable<T> Get<T>(this DbContext context) where T : class
        {
            return context.Set<T>().AsNoTracking();
        }

        internal static DbContext GetContext<T>(this DbSet<T> dbSet) where T : class
        {
            return dbSet.GetService<ICurrentDbContext>().Context;
        }

        internal static string GetTableName<T>(this IEntityType entityType) where T : class
        {
            var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
            var TableName = tableNameAnnotation.Value.ToString();
            return TableName;
        }

        internal static async Task<int> TruncateAsync<T>(this DbContext context, string schema = "dbo", CancellationToken ct = default) where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            string tableName = context.Set<T>().EntityType.GetTableName();
            System.Diagnostics.Trace.TraceInformation("TRUNCATE TABLE {0}.{1};", schema, tableName);
            return await context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {schema}.{tableName};", ct);
        }

        internal static async Task<int> DeleteAsync<T>(this DbContext context, string schema = "dbo", CancellationToken ct = default) where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            string tableName = context.Set<T>().EntityType.GetTableName();
            System.Diagnostics.Trace.TraceInformation("DELETE FROM {0}.{1};", schema, tableName);
            return await context.Database.ExecuteSqlRawAsync($"DELETE FROM {schema}.{tableName};", ct);
        }

        internal static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            if (dbSet.IsNotNullOrEmpty())
                dbSet.RemoveRange(dbSet.ToList());
        }

        //internal static async Task<T> AddOrUpdateAsync<T>(
        //    this DbContext context, T data, Expression<Func<T, bool>> predicate) where T : EntityBase
        //{
        //    T existing = null;
        //    var results = context.Get<T>().Where(predicate);
        //    if (results != null && results.Count() == 1)
        //        existing = await results.SingleOrDefaultAsync();
        //    await context.AddOrUpdateAsync(data, existing);
        //    return existing;
        //}

        internal static async Task<int> UpdateOrAddAsync<T>(
            this DbContext context, T data, Expression<Func<T, bool>> predicate, CancellationToken ct = default) where T : EntityBase
        {
            int updateCount = 0;
            var items = context.Get<T>().Where(predicate);
            if (items.IsNotNullOrEmpty())
            {
                foreach (var item in items)
                {
                    await context.AddOrUpdateAsync(data, item, ct);
                    updateCount++;
                }
            }
            else
            {
                await context.AddAsync(data, ct);
                updateCount++;
            }
            return updateCount;
        }

        internal static async Task AddOrUpdateAsync<T>(
            this DbContext context, T data, T existing = null, CancellationToken ct = default) where T : EntityBase
        {
            if (context != null && data != null)
            {
                data.ModifiedOn = DateTime.Now;
                if (existing != null)
                {
                    context.Entry(existing).CurrentValues.SetValues(data);
                }
                else
                {
                    data.CreatedOn = DateTime.Now;
                    await context.AddAsync(data, ct);
                }
            }
        }
    }
}
