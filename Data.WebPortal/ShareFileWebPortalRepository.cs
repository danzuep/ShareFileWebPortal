using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Common.Helpers;
using Data.WebPortal.Models;

/// <summary>
/// Data model generated using Microsoft.EntityFrameworkCore.Tools.
/// Temporarily set this project as the startup project before running one of the following commands (Package Manger Console below, .NET CLI example on the line below that).
/// Visual Studio -> Tools -> NuGet Package Manger -> Package Manger Console -> select this project from the dropdown to set it as the default project.
/// Scaffold-DbContext "Server=ShareFileWebPortalServer;Database=ShareFileWebPortalDatabase;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir "Models" -Force
/// Delete the default implementation of OnConfiguring as it is overwritten here.
/// </summary>
namespace Data.WebPortal
{
    //https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/
    public sealed class ShareFileWebPortalRepository : ShareFileWebPortalContext
    {
        #region Initialisation
        private const string DbName = "ShareFileWebPortal";
        private const string DateOnlyFormat = "yyyy'-'MM'-'dd"; // K not required
        private const string DateFormatLong = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff";
        private readonly ILogger _logger = LogProvider.GetLogger<ShareFileWebPortalRepository>();

        private static string _connectionString = "DataSource=app.db;Cache=Shared";
        public static string ConnectionString { private get => _connectionString; set => SetConnectionString(value); }
        public ShareFileWebPortalRepository(string connectionString) => ConnectionString = connectionString;
        public ShareFileWebPortalRepository(IConfiguration configuration) => ConnectionString = configuration.GetConnectionString(DbName);
        //public ShareFileWebPortalRepository(DbContextOptions<ShareFileWebPortalRepository> options) : base(options) { }

    private static void SetConnectionString(string connectionString)
        {
            string errorMessage =
                "ShareFileWebPortalRepository DB connection string configuration value {0}. " +
                @"Add 'ConnectionStrings:{1}' to 'appsettings.json' or as an Environment variable .";

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
                    .UseSqlServer(_connectionString, o =>
                    {
                        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        //o.EnableRetryOnFailure(1); //.EnableSensitiveDataLogging()
                    });
                _logger.LogDebug("{0} configured from connection string.", DbName);
            }
        }
        #endregion

        #region Rollover Accounts
        public async Task<IEnumerable<string>> GetAllRolloverAccountUids(CancellationToken ct = default)
        {
            return await RolloverAccounts
                .Where(o => o.SaveMethod > 0 &&
                    !string.IsNullOrEmpty(o.FileUid))
                .OrderByDescending(o => o.Id)
                .Select(o => o.FileUid)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<RolloverAccount>> GetRolloverAccountsByUid(
            string itemUid)
        {
            return await RolloverAccounts
                .Where(rac => rac.SaveMethod > 0 &&
                    rac.FileUid == itemUid)
                .OrderByDescending(rac => rac.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<RolloverAccount>> GetRolloverAccountsByUids(
            IEnumerable<string> itemUids)
        {
            return await RolloverAccounts
                .Where(rac => itemUids.Contains(rac.FileUid))
                .OrderByDescending(rac => rac.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task SetRolloverAccountUid(
            int? accountId, string fileUid, int saveMethod = 1)
        {
            if (accountId > 0)
            {
                //Database.Log = Console.WriteLine;
                var roa = RolloverAccounts.Find(accountId);

                if (roa == null) //throw new Exception("");
                    await SetRolloverAccountDisconnectedMethod(accountId, fileUid, saveMethod);

                roa.FileUid = fileUid;
                roa.SaveMethod = saveMethod;
                //await db.SaveChangesAsync();
            }
        }

        public async Task SetRolloverAccountDisconnectedMethod(
            int? accountId, string fileUid, int saveMethod = 1)
        {
            if (accountId > 0)
            {
                var roa = new RolloverAccount()
                {
                    Id = accountId.GetValueOrDefault(),
                    FileUid = fileUid,
                    SaveMethod = saveMethod,
                };

                //Database.Log = Console.WriteLine;
                RolloverAccounts.Attach(roa);
                Entry(roa).Property(roa.FileUid.GetType().Name).IsModified = true;
                Entry(roa).Property(roa.SaveMethod.GetType().Name).IsModified = true;
                //await db.SaveChangesAsync();
            }
        }

        public async Task<int> ClearRolloverAccountUids(
            string itemUid, int userId = 0)
        {
            int resultCount = 0;
            if (!string.IsNullOrEmpty(itemUid))
            {
                var accounts = await GetRolloverAccountsByUid(itemUid);
                resultCount = await ClearRolloverAccountsFileUids(accounts, userId);
            }
            return resultCount;
        }

        public async Task<IEnumerable<string>> ClearRolloverAccountUids(
            IEnumerable<string> folderUids, int userId = 0)
        {
            var results = Enumerable.Empty<string>();
            int resultCount = 0;
            if (folderUids?.Any() ?? false)
            {
                var accounts = await GetRolloverAccountsByUids(folderUids);
                results = accounts.Select(o => o.FileUid);
                resultCount = await ClearRolloverAccountsFileUids(accounts, userId);
            }
            return results;
        }

        public async Task<int> ClearRolloverAccountsFileUids(
            IEnumerable<RolloverAccount> rolloverAccounts, int userId = 0)
        {
            foreach (var account in rolloverAccounts)
            {
                account.SaveMethod = 0;
                account.FileUid = null;

                if (userId > 0)
                {
                    account.ModifiedById = userId;
                    account.ModifiedOn = DateTime.Now;
                }
            }

            return 0; //await db.SaveChangesAsync();
        }
        #endregion

        #region Rollover Object Accounts
        public async Task<IEnumerable<string>> GetRolloverObjectAccountUids(int? accountId)
        {
            List<string> guids = new List<string>();
            if (accountId > 0)
            {
                int id = accountId.GetValueOrDefault();

                var roa = await RolloverObjectAccounts
                    .Where(o => o.RolloverAccount.Id == id)
                    .OrderByDescending(o => o.Id)
                    .Include(o => o.RolloverAccount)
                    .Include(o => o.RolloverWorksheets)
                    .Select(rac => new
                    {
                            //RolloverId = rac.RolloverAccount.RolloverId,
                            //RolloverAccountId = rac.RolloverAccount.Id,
                            AccountSaveMethod = rac.RolloverAccount.SaveMethod,
                        AccountUid = rac.RolloverAccount.FileUid,
                            //MonthlyMovementId = rac.Id,
                            Worksheet = rac.RolloverWorksheets,
                    }).ToListAsync()
                    .ConfigureAwait(false);

                var accountUids = roa
                    .Where(filter => filter.AccountUid != null)
                    //.Where(filter => filter.AccountSaveMethod > 0)
                    .GroupBy(key => key.AccountUid)
                    .Select(group => group.Key);

                if (accountUids?.Any() ?? false)
                {
                    guids.AddRange(accountUids);
                }

                var worksheetUids = roa.SelectMany(o => o.Worksheet)
                    .Where(filter => filter.FileUid != null)
                    //.Where(o => o.SaveMethod > 0)
                    .Select(o => o.FileUid);

                if (worksheetUids?.Any() ?? false)
                {
                    guids.AddRange(worksheetUids);
                }
            }
            return guids;
        }
        #endregion

        #region Documents
        public async Task<IEnumerable<Document>> GetSscPortalDocuments()
        { 
            return await Documents
                .Where(o => o.RawData != null)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<int> AddDocumentsAsync(IEnumerable<Document> docs)
        {
            Documents.AddRange(docs);
            //return await db.SaveChangesAsync();
            return 0;
        }

        public async Task<int> ClearDocumentData(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var doc = Documents.Find(id);
                if (doc == null)
                    throw new Exception($"ShareFileDb.Document.ID {id} not found.");
                doc.RawData = null;
                doc.ModifiedOn = DateTime.Now;
            }
            return 0; //await db.SaveChangesAsync();
        }
        #endregion
    }

    #region Enums
    #endregion
}
