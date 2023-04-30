using System;
using System.Linq;
//using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Data.ShareFile.Models;
using Data.ShareFile.Extensions;
using Data.ShareFile.DbFirstModels;

/// <summary>
/// Data model generated using Microsoft.EntityFrameworkCore.Tools.
/// Temporarily set this project as the startup project before running one of the following commands (Package Manger Console below, .NET CLI example on the line below that).
/// Visual Studio -> Tools -> NuGet Package Manger -> Package Manger Console -> select this project from the dropdown to set it as the default project.
/// Scaffold-DbContext Name=ConnectionStrings:ShareFileDb Microsoft.EntityFrameworkCore.SqlServer -OutputDir "Models" -DataAnnotations -Force -Tables Application, ApplicationUser, Document, Company, PublicHoliday, SecurityCompany, SecurityCompany_New, SecurityCompanyPermission, SecurityUserCompany, SharefileAuth, ShareFileGroup, ShareFileGroupMember, AccessControl, Group, Item, Principal, User
/// Delete the default implementation of OnConfiguring as it is overwritten here.
/// https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/managing?tabs=dotnet-core-cli#add-a-migration
/// Add-Migration InitialCreate -Context ShareFilePortalContext -Project Data.ShareFile
/// https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=vs#sql-scripts
/// Script-Migration -Context ShareFilePortalContext
/// </summary>
namespace Data.ShareFile
{
    //https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/
    public sealed class ShareFileRepository : ShareFilePortalContext
    {
        #region Initialisation
        private const string DbName = "ShareFileDb";
        private const int GuidLength = 36;
        private const string DateOnlyFormat = "yyyy'-'MM'-'dd"; // K not required
        private const string DateFormatLong = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff";
        private readonly ILogger _logger = Common.Helpers.LogProvider.GetLogger<ShareFileRepository>();

        private static string _connectionString = "DataSource=app.db;Cache=Shared";
        public static string ConnectionString { private get => _connectionString; set => SetConnectionString(value); }

        public ShareFileRepository(string connectionString) => ConnectionString = connectionString;

        public ShareFileRepository(IConfiguration configuration) => ConnectionString = configuration.GetConnectionString(DbName);

        //public ShareFileRepository(DbContextOptions<ShareFileRepository> options) : base(options) { _logger.LogDebug("{0} options configured.", DbName); }

        private static void SetConnectionString(string connectionString)
        {
            string errorMessage = $"{DbName} DB connection string configuration value {{0}}. " +
                @"Add 'ConnectionStrings:{1}' to 'appsettings.json' or as an Environment variable.";

            if (string.IsNullOrEmpty(connectionString))
                throw new NullReferenceException(string.Format(errorMessage, "not found", DbName));
            else if (!connectionString.StartsWith("Server="))
                throw new ArgumentException(string.Format(errorMessage, "is encrypted", DbName));
            else
                _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options//.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                       //.UseLazyLoadingProxies()
                    .UseSqlServer(_connectionString, o =>
                    {
                        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        //o.EnableRetryOnFailure(1); //.EnableSensitiveDataLogging()
                    });
                _logger.LogDebug("{0} configured from connection string.", DbName);
            }
        }
        #endregion

        #region Users and Principals
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers(CancellationToken ct = default)
        {
            return await ApplicationUsers
                .Where(o => o.Enabled == true)
                .OrderBy(o => o.LastName)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public IQueryable<DbPrincipal> GetShareFilePrincipals() =>
            Principals.AsNoTracking().Where(g => !g.IsDeleted).OrderBy(g => g.Name);

        public async Task<IEnumerable<DbPrincipal>> GetShareFilePrincipals(CancellationToken ct = default)
        {
            return await GetShareFilePrincipals().ToListAsync(ct).ConfigureAwait(false);
        }
        #endregion

        public IQueryable<SfItem> GetTrackedShareFileItems() =>
            Items.Where(g => !g.IsDeleted).OrderBy(g => g.SemanticPath);

        public IQueryable<SfItem> GetShareFileItems() =>
            Items.AsNoTracking().Where(g => !g.IsDeleted).OrderBy(g => g.SemanticPath);

        public async Task<IEnumerable<SfItem>> GetShareFileItemsAsync(IQueryable<SfItem> sfItems, CancellationToken ct = default)
        {
            return await sfItems.ToListAsync(ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<SfAccessControl>> GetShareFileAccessControls(string userUid, CancellationToken ct = default)
        {
            return await AccessControls.AsNoTracking()
                .Where(a => !a.IsDeleted && a.PrincipalId == userUid)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public IQueryable<SfItem> GetShareFileItems(IEnumerable<string> itemUids)
        {
            return Items.AsNoTracking()
                .Where(g => !g.IsDeleted && itemUids.Contains(g.Uid))
                .OrderBy(g => g.SemanticPath);
        }

        public async Task<IList<SfItem>> GetShareFileItemsAsync(IEnumerable<string> itemUids, CancellationToken ct = default)
        {
            return await GetShareFileItems(itemUids).ToListAsync(ct).ConfigureAwait(false);
        }

        public IQueryable<SfAccessControl> GetShareFileAccessControls(IEnumerable<string> itemUids, IEnumerable<string> principalUids)
        {
            return AccessControls.AsNoTracking()
                .Where(a => !a.IsDeleted &&
                    itemUids.Contains(a.ItemId) &&
                    principalUids.Contains(a.PrincipalId))
                .Include(o => o.Principal)
                .Include(o => o.Item)
                .OrderBy(o => o.Item.SemanticPath)
                .ThenBy(o => o.Item.Name);
        }

        public async Task<IList<SfAccessControl>> GetShareFileAccessControlsAsync(IEnumerable<string> itemUids, IEnumerable<string> principalUids, CancellationToken ct = default)
        {
            return await GetShareFileAccessControls(itemUids, principalUids).ToListAsync(ct).ConfigureAwait(false);
        }

        public async Task<IList<SfAccessControl>> GetShareFileAccessControlsAsync(IQueryable<SfAccessControl> accessControls, IEnumerable<string> itemUids, CancellationToken ct = default)
        {
            return await accessControls
                .Where(a => itemUids.Contains(a.ItemId))
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IList<SfAccessControl>> GetShareFileAccessControlsAsync(
            IQueryable<SfAccessControl> accessControls, IEnumerable<SfItem> items, CancellationToken ct = default)
        {
            return await accessControls
                .Where(a => items.Any(i => i.Uid == a.ItemId))
                //.Select(a => a with { FolderPath = b.ItemPath})
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IList<SfAccessControl>> GetShareFileAccessControlsAsync(IEnumerable<SfItem> items, CancellationToken ct = default)
        {
            return await GetShareFileItemAccessControls(items)
                //.Select(a => a with { ItemPath = items.Where(i => i.Uid == a.ItemId).FirstOrDefault().SemanticPath })
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public IQueryable<SfAccessControl> GetShareFileItemAccessControls(IEnumerable<SfItem> items)
        {
            return AccessControls.AsNoTracking()
                .Where(a => items.Any(i => i.Uid == a.ItemId));
        }

        public IQueryable<SfAccessControl> GetShareFilePrincipalItemAccessControls(string principalUid)
        {
            return AccessControls.AsNoTracking()
                .Where(a => !a.IsDeleted && a.PrincipalId == principalUid);
        }

        public IQueryable<SfAccessControl> GetShareFilePrincipalItemAccessControls(IEnumerable<string> principalUids)
        {
            return AccessControls.AsNoTracking()
                .Where(a => !a.IsDeleted && principalUids.Contains(a.PrincipalId));
        }

        public IQueryable<SfAccessControl> GetShareFilePrincipalAccessControls(string principalUid)
        {
            return AccessControls.AsNoTracking()
                .Where(a => !a.IsDeleted &&
                    a.PrincipalId == principalUid &&
                    a.CanView == true)
                .Include(o => o.Principal)
                .Include(o => o.Item);
        }

        public IQueryable<SfAccessControl> GetShareFilePrincipalAccessControls(IEnumerable<string> principalUids)
        {
            return AccessControls.AsNoTracking()
                .Where(a => !a.IsDeleted &&
                    principalUids.Contains(a.PrincipalId))
                .Include(o => o.Principal)
                .Include(o => o.Item);
        }

        public IQueryable<ShareFileDbGroup> GetShareFileUserGroupUids(string userUid)
        {
            return ShareFileGroups.AsNoTracking()
                .Where(g => g.ShareFileGroupMembers.Any(m => m.Uid == userUid))
                .Distinct();
        }

        public async Task<IList<string>> GetShareFileUserItemsAsync(string userUid, CancellationToken ct = default)
        {
            return await GetShareFilePrincipalItemAccessControls(userUid)
                .Select(a => a.ItemId).Distinct().ToListAsync(ct).ConfigureAwait(false);
        }

        public async Task<IList<string>> GetShareFileGroupItemsAsync(IEnumerable<string> groupUids, CancellationToken ct = default)
        {
            //var groupUids = groups.Select(a => a.Uid).Distinct();
            return await GetShareFilePrincipalItemAccessControls(groupUids)
                .Select(a => a.ItemId).Distinct().ToListAsync(ct).ConfigureAwait(false);
        }

        public async Task<int> SetSfAsync<T>(IEnumerable<T> entities, int updateLimit = 100, CancellationToken ct = default) where T : EntityUidBase
        {
            int updateCount = 0;
            if (entities.IsNotNullOrEmpty())
            {
                int totalCount = entities.Count();
                if (totalCount > updateLimit)
                    throw new DbUpdateException($"Too many {nameof(entities)} (> {updateLimit})");

                string tableName = typeof(T).Name;
                try
                {
                    using var transaction = Database.BeginTransaction();
                    foreach (var entity in entities)
                        updateCount += await this.UpdateOrAddAsync(
                            entity, db => db.Uid == entity.Uid);
                    int changeCount = await SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogDebug("Updated or added {0} of {1} records to {2} ({3} changed)",
                        updateCount, totalCount, tableName, changeCount);
                }
                catch (InvalidOperationException ex) { _logger.LogWarning(ex, "Database connection disposed prematurely."); }
                catch (Exception ex)
                {
                    var e = ex.InnerException ?? ex;
                    _logger.LogError(e, "Failed to update {0} record in the database. " +
                        "Transaction has been rolled back automatically on disposal.", tableName);
                    System.Diagnostics.Debugger.Break();
                }
            }
            return updateCount;
        }

        public async Task<int> AddOverwriteAsync<T>(IEnumerable<T> entities, CancellationToken ct = default) where T : class
        {
            int updateCount = 0;
            if (entities.IsNotNullOrEmpty() && !ct.IsCancellationRequested)
            {
                string tableName = typeof(T).Name;
#if DEBUG
                if (typeof(T) is EntityUidBase && entities.Any(e => (e as EntityUidBase)?.Uid?.Length > 36))
                    throw new ArgumentOutOfRangeException("Uid", "GUID length exceeds 36 chars");
#endif
                try
                {
                    using var transaction = Database.BeginTransaction();
                    //await this.TruncateAsync<T>("dbo"); // faster but requires ALTER permissions
                    await this.DeleteAsync<T>("dbo"); // slower but only requires DELETE permissions
                    await AddRangeAsync(entities);
                    int changeCount = await SaveChangesAsync();
                    await transaction.CommitAsync();
                    int totalCount = entities.Count();
                    _logger.LogDebug("Added {0} of {1} records to {2} ({3} changed)",
                        updateCount, totalCount, tableName, changeCount);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Database connection disposed prematurely.");
                }
                catch (Exception ex)
                {
                    var e = ex.InnerException ?? ex;
                    _logger.LogError(e, "Failed to update {0} record in the database. " +
                        "Transaction has been rolled back automatically on disposal.", tableName);
                    System.Diagnostics.Debugger.Break();
                }
            }
            return updateCount;
        }

        #region ShareFile Group
        public async Task<IEnumerable<ShareFileDbGroup>> GetRepoShareFileGroups(CancellationToken ct = default)
        {
            var repo = new AtomicRepository<ShareFileDbGroup>(this);
            return await repo.GetAll();
        }

        //public async Task<int> SetRepoShareFileGroups(IEnumerable<ShareFileGroup> items, CancellationToken ct = default)
        //{
        //    var repo = new AtomicRepository<ShareFileGroup>(this);
        //    return await repo.SaveRange(items);
        //}

        public async Task<int> SetRepoShareFileGroup(ShareFileDbGroup group, CancellationToken ct = default)
        {
            int updateCount = 0;
            if (group != null)
            {
                string groupTableName = nameof(ShareFileDbGroup);
                string memberTableName = nameof(ShareFileDbGroupMember);
                try
                {
                    using var transaction = Database.BeginTransaction();
                    var existing = await ShareFileGroups.AsNoTracking()
                        .Where(g => g.Uid == group.Uid).FirstOrDefaultAsync();
                    await this.AddOrUpdateAsync(group, existing);
                    int addCount = 0;
                    int changeCount = 0;
                    int memberCount = group.ShareFileGroupMembers.Count;
                    if (group.ShareFileGroupMembers != null)
                    {
                        foreach (var member in group.ShareFileGroupMembers)
                        {
                            //try
                            //{
                                if (member.Group is null)
                                    member.Group = group;
                                //else if (group?.Id > 0)
                                //    member.GroupId = group.Id;
                                //else
                                //    System.Diagnostics.Debugger.Break();
                                addCount += await this.UpdateOrAddAsync(member, m => m.Uid == member.Uid);
                            //    changeCount += await SaveChangesAsync();
                            //}
                            //catch (Exception ex)
                            //{
                            //    var e = ex.InnerException ?? ex;
                            //    _logger.LogError(e, "Failed to update {0} record in the database. " +
                            //        "Transaction has been rolled back automatically on disposal.", memberTableName);
                            //    System.Diagnostics.Debugger.Break();
                            //}
                        }
                    }
                    changeCount += await SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogDebug("Added {0} of {1} group members to {2} ({3} changed)",
                        addCount, memberCount, memberTableName, changeCount);
                }
                catch (InvalidOperationException ex) { _logger.LogWarning(ex, "Database connection disposed prematurely."); }
                catch (Exception ex)
                {
                    var e = ex.InnerException ?? ex;
                    _logger.LogError(e, "Failed to update {0} record in the database. " +
                        "Transaction has been rolled back automatically on disposal.", groupTableName);
                    System.Diagnostics.Debugger.Break();
                }
            }
            return updateCount;
        }

        public async Task<int> SetRepoShareFileGroups(IEnumerable<ShareFileDbGroup> items, CancellationToken ct = default)
        {
            int updateCount = 0;
            if (items != null)
            {
                string tableName = nameof(ShareFileDbGroup);
                int totalItemCount = items.Count();
                try
                {
                    using var transaction = Database.BeginTransaction();
                    int addCount = 0;
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            addCount += await this.UpdateOrAddAsync(item, g => g.Uid == item.Uid);
                        }
                    }
                    int changeCount = await SaveChangesAsync();
                    await transaction.CommitAsync();
                    updateCount = totalItemCount;
                    _logger.LogDebug("Updated or added {0} of {1} records to {2} ({3} changed)",
                        addCount, totalItemCount, tableName, changeCount);
                }
                catch (InvalidOperationException ex) { _logger.LogWarning(ex, "Database connection disposed prematurely."); }
                catch (Exception ex)
                {
                    var e = ex.InnerException ?? ex;
                    _logger.LogError(e, "Failed to update {0} record in the database. " +
                        "Transaction has been rolled back automatically on disposal.", tableName);
                    System.Diagnostics.Debugger.Break();
                }
            }
            return updateCount;
        }

        public async Task<int> SetSfAccessControl(IEnumerable<SfAccessControl> accessControls, CancellationToken ct = default)
        {
            int updateCount = 0;
            if (accessControls.IsNotNullOrEmpty())
            {
                string tableName = nameof(ShareFileDbGroup);
                try
                {
                    using var transaction = Database.BeginTransaction();
                    //await AddRangeAsync(accessControls);
                    //var items = accessControls.Where(a => a.Item != null).Select(a => a.Item).Distinct();
                    //foreach (var item in items)
                    //{
                    //    item.Children = null;
                    //    item.AccessControls = null;
                    //    await AddAsync(item);
                    //}
                    int changeCount1 = await SaveChangesAsync();
                    foreach (var ac in accessControls)
                    {
                        //ac.Item = null;
                        //ac.Principal = null;
                        await AddAsync(ac);
                        //updateCount += await this.UpdateOrAddAsync(ac, db => db.Id == ac.Id);
                    }
                    //var items = accessControls.Where(a => a.Item != null).Select(a => a.Item).Distinct();
                    //foreach (var item in items)
                    //{
                    //    item.AccessControls = null;
                    //    if (item.Parent != null && Items.Any(p => p.Id == item.Parent.Id))
                    //        item.Parent = null;
                    //}
                    //foreach (var ac in accessControls)
                    //{
                    //    ac.Item = null;
                    //    if (ac.Principal != null && Principals.Any(p => p.Id == ac.Principal.Id))
                    //        ac.Principal = null;
                    //    ac.Principal = null;
                    //    updateCount += await this.UpdateOrAddAsync(ac, db => db.Id == ac.Id);
                    //}
                    //foreach (var ac in accessControls)
                    //{
                    //    if (ac.Principal != null)
                    //        await this.UpdateOrAddAsync(ac.Principal, db => db.Uid == ac.Principal.Uid);
                    //    //if (ac.User != null)
                    //    //    await this.UpdateOrAddAsync(ac.User, db => db.Uid == ac.User.Uid);
                    //    //if (ac.Group != null)
                    //    //    await this.UpdateOrAddAsync(ac.Group, db => db.Uid == ac.Group.Uid);
                    //    ac.Item = null;
                    //    ac.Principal = null;
                    //    if (ac.Uid.Length > GuidLength)
                    //        ac.Uid = ac.Uid.GetHashCode().ToString();
                    //    updateCount += await this.UpdateOrAddAsync(ac, db => db.Uid == ac.Uid);
                    //}
                    int changeCount = await SaveChangesAsync();
                    await transaction.CommitAsync();
                    int totalCount = accessControls.Count();
                    _logger.LogDebug("Updated or added {0} of {1} records to {2} ({3} changed)",
                        updateCount, totalCount, tableName, changeCount);
                }
                catch (InvalidOperationException ex) { _logger.LogWarning(ex, "Database connection disposed prematurely."); }
                catch (Exception ex)
                {
                    var e = ex.InnerException ?? ex;
                    _logger.LogError(e, "Failed to update {0} record in the database. " +
                        "Transaction has been rolled back automatically on disposal.", tableName);
                    System.Diagnostics.Debugger.Break();
                }
            }
            return updateCount;
        }

        public async Task<IEnumerable<ShareFileDbGroup>> GetShareFileGroups(CancellationToken ct = default)
        {
            return await ShareFileGroups
                .Where(g => !g.IsDeleted)
                .OrderBy(g => g.Name)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroup>> GetFilteredShareFileGroups(CancellationToken ct = default)
        {
            return await ShareFileGroups
                .Where(g => !g.IsDeleted &&
                    g.ShareFileGroupMembers.Any())
                .OrderBy(g => g.Name)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroup>> GetFilteredShareFileGroups(string prefix = "", CancellationToken ct = default)
        {
            return await ShareFileGroups
                .Where(g => !g.IsDeleted &&
                    g.IsShared == true &&
                    g.ShareFileGroupMembers.Any() &&
                    g.Name.StartsWith(prefix))
                .OrderBy(g => g.Name)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroup>> GetAllShareFileGroups(CancellationToken ct = default)
        {
            return await ShareFileGroups
                .Where(o => !o.IsDeleted)
                .Include(o => o.ShareFileGroupMembers)
                .OrderBy(o => o.Name)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroup>> GetShareFileGroupFromUid(string groupUid, CancellationToken ct = default)
        {
            return await ShareFileGroups
                .Where(o => !o.IsDeleted &&
                    //!string.IsNullOrEmpty(o.Uid) &&
                    o.Uid == groupUid)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroup>> GetShareFileMemberGroups(ShareFileDbGroupMember member, CancellationToken ct = default)
        {
            return await ShareFileGroups
                .Include(o => o.ShareFileGroupMembers)
                .Where(o => !o.IsDeleted &&
                    o.ShareFileGroupMembers.Contains(member))
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroup>> GetShareFileMemberGroupsFromUid(string memberUid, CancellationToken ct = default)
        {
            return await ShareFileGroups
                .Include(o => o.ShareFileGroupMembers)
                .Where(o => !o.IsDeleted &&
                    o.ShareFileGroupMembers.Any(m => m.Uid == memberUid))
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }
        #endregion

        #region ShareFile Group Member
        public async Task<IEnumerable<ShareFileDbGroupMember>> GetAllShareFileGroupMembers(CancellationToken ct = default)
        {
            return await ShareFileGroupMembers
                .Where(o => !o.IsDeleted)
                .Include(o => o.Group)
                .OrderBy(o => o.LastName)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroupMember>> GetShareFileGroupMembersFromUid(string groupUid, CancellationToken ct = default)
        {
            return await ShareFileGroupMembers
                .Where(o => o.Group.Uid == groupUid)
                .Include(o => o.Group)
                //.OrderBy(o => o.LastName)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroupMember>> GetShareFileGroupMembersFromUid2(string groupUid, CancellationToken ct = default)
        {
            return await ShareFileGroupMembers
                .Where(o => !o.IsDeleted &&
                    //!string.IsNullOrEmpty(o.Uid) &&
                    o.Uid == groupUid)
                .Include(o => o.Group)
                .OrderBy(o => o.LastName)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroupMember>> GetShareFileGroupMembers(string id, CancellationToken ct = default)
        {
            return await ShareFileGroupMembers
                .Where(o => !o.IsDeleted &&
                    o.Group.Uid == id)
                .Include(o => o.Group)
                .OrderBy(o => o.LastName)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShareFileDbGroupMember>> GetShareFileGroupMembers(int id, CancellationToken ct = default)
        {
            return await ShareFileGroupMembers
                .Where(o => !o.IsDeleted &&
                    o.GroupId == id)
                .Include(o => o.Group)
                .OrderBy(o => o.LastName)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        //public async Task<ShareFileDbGroup> GetShareFileGroupMembers(ShareFileDbGroup group, CancellationToken ct = default)
        //{
        //    if (group != null)
        //    {
        //        group.ShareFileGroupMembers = await ShareFileGroupMembers
        //            .Where(o => !o.IsDeleted &&
        //                o.GroupId == group.Id)
        //            .Include(o => o.Group)
        //            .OrderBy(o => o.LastName)
        //            .ToListAsync(ct)
        //            .ConfigureAwait(false);
        //    }
        //    return group ?? new ShareFileDbGroup();
        //}

        public async Task<IEnumerable<string>> GetNonShareFileGroupMembers(IEnumerable<string> groupUids, CancellationToken ct = default)
        {
            return groupUids.IsNullOrEmpty() ? Array.Empty<string>() :
                await ViewShareFileGroupMembers
                    .Where(o => !groupUids.Contains(o.Uid))
                    .Select(o => o.Uid)
                    .ToListAsync(ct)
                    .ConfigureAwait(false);
        }
        #endregion
    }

    #region Enums
    #endregion
}
