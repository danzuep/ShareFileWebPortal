using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ShareFile.Api.Client;
using ShareFile.Api.Client.Models;
using ShareFile.Api.Client.Transfers;
using ShareFile.Api.Client.Exceptions;
using ShareFile.Api.Client.Security.Authentication.OAuth2;
using ShareFile.Api.Client.Extensions;
using ShareFile.Api.Client.Requests;
using Common.Helpers;

namespace ShareFile.Api.Helpers.Extensions
{
    public static class ShareFileClientExtensions
    {
        private static ILogger _logger = LogProvider.GetLogger(nameof(ShareFileClientExtensions));
        //private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions() { WriteIndented = true };

        private static IDictionary<string, object> _locks = new Dictionary<string, object>();
        public static int FolderDepthLimit { get; set; } = -1;

        private static object GetLock(string name)
        {
            if (!_locks.ContainsKey(name))
                _locks.Add(name, new object());
            return _locks[name];
        }

        private static void ReleaseLock(string name)
        {
            if (_locks.ContainsKey(name))
                _locks.Remove(name);
        }

        #region Initialisation
        public static OAuthService GetOAuthService(this IShareFileClient sfClient, string clientId, string clientSecret)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException(nameof(clientId),
                    "You must provide the client ID");
            else if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentNullException(nameof(clientSecret),
                    "You must provide the client secret");
            return new OAuthService(sfClient ?? ShareFileApi.GetDefault(), clientId, clientSecret);
        }

        public static async Task<OAuthToken> GetOAuthTokenAsync(this OAuthService sfService, NetworkCredential credential, string controlPlane = "sharefile.com")
        {
            if (sfService is null)
                throw new ArgumentNullException(nameof(sfService));
            else if (string.IsNullOrWhiteSpace(credential?.UserName) ||
                string.IsNullOrWhiteSpace(credential?.Password) ||
                string.IsNullOrWhiteSpace(credential?.Domain))
                throw new ArgumentNullException(nameof(credential),
                    "You must provide a username, password and subdomain");

            var oAuthToken = await sfService.PasswordGrantAsync(
                credential.UserName, credential.Password, credential.Domain, controlPlane);

            sfService.ShareFileClient.AddOAuthCredentials(oAuthToken);
            sfService.ShareFileClient.BaseUri = oAuthToken.GetUri();

            return oAuthToken;
        }

        internal static async Task<OAuthToken> GetOAuthTokenAsync(this OAuthService sfService, OAuthToken sfToken)
        {
            if (sfService is null)
                throw new ArgumentNullException(nameof(sfService));
            else if (sfService.ShareFileClient is null)
                throw new ArgumentNullException(nameof(sfService.ShareFileClient));
            else if (sfToken is null)
                throw new ArgumentNullException(nameof(sfToken));

            var refreshedOAuthToken = await sfService.RefreshOAuthTokenAsync(sfToken);
            System.Diagnostics.Trace.TraceInformation("Session token refreshed until {0:d}", refreshedOAuthToken.ExpirationDate);

            return refreshedOAuthToken;
        }

        public static async Task<IShareFileClient> RefreshSfClientAsync(this OAuthService sfService, OAuthToken sfToken)
        {
            var refreshedOAuthToken = await sfService.RefreshOAuthTokenAsync(sfToken);
            return sfService.ShareFileClient;
        }

        public static async Task<OAuthToken> StartSessionAsync(this OAuthService sfService, NetworkCredential credential)
        {
            var token = await sfService.GetOAuthTokenAsync(credential);
            //await sfService.ShareFileClient.StartSessionAsync();
            System.Diagnostics.Trace.TraceInformation("Session started, authenticated as {0}", credential.UserName);
            return token;
        }

        internal static async Task StartSessionAsync(this IShareFileClient sfClient)
        {
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            var session = await sfClient.Sessions.Login().Expand("Principal").ExecuteAsync();
            System.Diagnostics.Trace.TraceInformation("Session started, authenticated as {0}", session.Principal.Email);
        }

        internal static void EndSession(this IShareFileClient sfClient)
        {
            //var session = await sfClient?.Session.Logout().ExecuteAsync();
            sfClient?.ClearCredentialsAndCookies();
        }
        #endregion

        #region Child Items
        private static async Task<IEnumerable<Item>> GetChildrenAsync(this IShareFileClient sfClient, string itemId)
        {
            ODataFeed<Item> items = null;
            if (sfClient != null && !string.IsNullOrWhiteSpace(itemId))
            {
                var uri = sfClient.Items.GetEntityUriFromId(itemId);
                items = await sfClient.Items.GetChildren(uri).ExecuteAsync();
            }
            return items.GetValues();
        }

        public static async Task<IEnumerable<Item>> GetAllChildItemsAsync(this IShareFileClient sfClient, IEnumerable<Item> items)
        {
            if (items == null || !items.Any()) return Enumerable.Empty<Item>();
            var tasks = items.Select(o => sfClient.Items.GetChildren(o.url).ExecuteAsync());
            var results = await Task.WhenAll(tasks);
            return results.Where(o => o != null).SelectMany(o => o.Feed);
        }

        public static async Task<IEnumerable<Item>> GetFlattenedFolderItems(this IShareFileClient sfClient, Folder parentFolder)
        {
            var childItems = new List<Item>();
            if (parentFolder?.Children?.Count() > 0)
            {
                var childFolders = parentFolder.Children.Where(c => c is Folder);
                //childItems.AddRange(childFolders);
                //var childSubfolders = await sfClient.GetAllChildItemsAsync(childFolders);
                foreach (var childItem in childFolders)
                {
                    childItems.Add(childItem);
                    var childFolder = await sfClient.GetFolderFromUidAsync(childItem.Id);
                    var subfolderItems = await sfClient.GetFlattenedFolderItems(childFolder);
                    childItems.AddRange(subfolderItems);
                }
            }
            return childItems;
        }

        public static async Task<IList<Folder>> GetRecursiveFolderAccess(this IShareFileClient sfClient,
            Folder folder, ICollection<string> foldersToSkip = null, bool logFolders = true, bool logFiles = false, int currentFolderDepth = 0, CancellationToken ct = default)
        {
            var folders = new List<Folder>();
            if (folder != null && folder.AccessControls != null && !ct.IsCancellationRequested)
            {
                string blankSpace = new string(' ', currentFolderDepth);
                if (logFolders)
                    _logger.LogDebug($"{blankSpace}+{folder.Name} ({folder.FileCount})");
                folder.AccessControls.ActionEach(a => a.Item = folder);
                folders.TryAdd(folder);
                if (foldersToSkip.IsNotNullOrEmpty() && foldersToSkip.Contains(folder.Id))
                    foldersToSkip.Remove(folder.Id);
                else if (foldersToSkip.IsNotNullOrEmpty() && foldersToSkip.Contains(folder.Name))
                    foldersToSkip.Remove(folder.Name);
                else if (currentFolderDepth++ < FolderDepthLimit || FolderDepthLimit < 0)
                {
                    var childFolders = folder.Children.Where(c => c is Folder);//.Select(c => c as Folder);
                    var folderPaths = new List<string>() { folder.SemanticPath, folder.Name };
                    var folderAccessTasks = childFolders.Select(item =>
                        sfClient.GetFolderAccessControls(item.Id, folderPaths));
                    var childFiles = folder.Children.Where(c => c is Client.Models.File);
                    if (logFiles && childFiles != null)
                        foreach (var childItem in childFiles)
                            _logger.LogDebug($"{blankSpace}--{childItem.Name} ({childItem.FileSizeInKB} KB)");
                    var childFolderAccess = await Task.WhenAll(folderAccessTasks);
                    foreach (var childFolder in childFolderAccess)
                    {
                        if (ct.IsCancellationRequested)
                            break;
                        var subfolders = await sfClient.GetRecursiveFolderAccess(
                            childFolder, foldersToSkip, logFolders, logFiles, currentFolderDepth, ct);
                        folders.TryAddRange(subfolders);
                    }
                }
            }
            return folders;
        }

        public static async Task<Folder> GetFolderAccessControlsFromPath(this IShareFileClient sfClient, string itemPath, CancellationToken ct = default)
        {
            Folder folder = null;
            if (!string.IsNullOrEmpty(itemPath) && !ct.IsCancellationRequested)
            {
                try
                {
                    var query = sfClient.Items.ByPath(itemPath);
                    var paths = new string[] { itemPath };
                    folder = await sfClient.GetFolderAccessControls(query, paths, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ShareFile API failed to return Folder AccessControls for {0}.", itemPath);
                }
            }
            return folder;
        }

        public static async Task<Folder> GetFolderAccessControls(this IShareFileClient sfClient, string itemUid, IEnumerable<string> paths = null, CancellationToken ct = default)
        {
            Folder folder = null;
            if (!string.IsNullOrEmpty(itemUid) && !ct.IsCancellationRequested)
            {
                try
                {
                    var folderUri = sfClient.Items.GetEntityUriFromId(itemUid);
                    //System.Diagnostics.Trace.TraceInformation("GET {0}://{1}.sharefile.com/home/shared/{2}",
                    //    folderUri.Scheme, folderUri.Host.Split('.').FirstOrDefault(), itemUid);
                    var query = sfClient.Items.Get(folderUri);
                    folder = await sfClient.GetFolderAccessControls(query, paths, ct);
                }
                catch (Exception ex)
                {
                    string subdomain = sfClient?.BaseUri?.Host?.Split('.').FirstOrDefault() ?? "";
                    var url = string.Format("https://{0}.sharefile.com/home/shared/{1}", subdomain, itemUid);
                    _logger.LogError(ex, "ShareFile API failed to return Folder AccessControls for {0}.", url);
                }
            }
            return folder;
        }

        public static async Task<Folder> GetFolderAccessControls(this IShareFileClient sfClient, IQuery<Item> query, IEnumerable<string> paths = null, CancellationToken ct = default)
        {
            Folder folder = null;
            if (query != null && !ct.IsCancellationRequested)
            {
                var item = await query.Expand("AccessControls").Expand("Children").ExecuteAsync(ct);
                folder = item as Folder;
                if (folder != null)
                {
                    if (paths.IsNullOrEmpty())
                    {
                        const string Folders = "Folders";
                        var breadcrumbs = await sfClient.Items.GetBreadcrumbs(item.url).SelectAndExecute(f => f.Name, ct);
                        paths = breadcrumbs.Count > 0 && breadcrumbs.FirstOrDefault() == Folders ? breadcrumbs.Skip(1) : breadcrumbs;
                    }
                    folder.SemanticPath = Path.Combine(paths.ToArray()).Replace('\\', '/');
                    //System.Diagnostics.Trace.TraceInformation("Folder loaded: {0}, containts {1} ({2})",
                    //    folder.Name, folder.FileCount, folder.SemanticPath);
                }
            }
            return folder;
        }

        public static async Task<Folder> GetFolderAccessControlsAsync(this IShareFileClient sfClient, string folderUid, CancellationToken ct = default)
        {
            folderUid = folderUid?.Trim();
            bool isFolderPath = !string.IsNullOrEmpty(folderUid) &&
                (folderUid.Contains('/') || folderUid.Contains('\\') || folderUid.Contains(' '));
            return isFolderPath ?
                await sfClient.GetFolderAccessControlsFromPath(folderUid, ct) :
                await sfClient.GetFolderAccessControls(folderUid, null, ct: ct);
        }

        public static async Task<IList<Folder>> GetFolderAccessControlsAsync(this IShareFileClient sfClient, string folderToProcess = "allshared", int folderDepthLimit = -1, IEnumerable<string> foldersToSkip = null, CancellationToken ct = default)
        {
            IList<Folder> folders = null;
            if (!ct.IsCancellationRequested)
            {
                var folder = await sfClient.GetFolderAccessControlsAsync(folderToProcess, ct);
                FolderDepthLimit = folderDepthLimit;
                var skipFolders = foldersToSkip?.Select(f => f.Trim()).ToList();
                if (skipFolders.IsNullOrEmpty()) skipFolders = null;
                folders = await sfClient.GetRecursiveFolderAccess(folder, skipFolders, ct: ct);
            }
            return folders ?? Array.Empty<Folder>();
        }

        //public static async Task<IEnumerable<Item>> GetFlattenedFolderItems(this IShareFileClient sfClient, IEnumerable<Item> folders)
        //{
        //    var childItems = new List<Item>();
        //    if (folders != null && folders.Any())
        //    {
        //        foreach (var parentItem in folders)
        //        {
        //            if (parentItem is Folder parentFolder)
        //            {
        //                var folderItems = await sfClient.GetAllChildItemsAsync(parentFolder.Children);
        //                childItems.AddRange(folderItems);
        //                var subfolderItems = await sfClient.GetFlattenedFolderItems(folderItems);
        //                childItems.AddRange(subfolderItems);
        //            }
        //        }
        //    }
        //    return childItems;
        //}

        public static async Task<ODataFeed<Group>> GetAllGroupsAndMembersAsync(this IShareFileClient sfClient, CancellationToken ct = default)
        {
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            var groupFeed = await sfClient.Groups.Get().Expand("Contacts").ExecuteAsync(ct);
            System.Diagnostics.Trace.TraceInformation("{0} groups loaded", groupFeed.count);
            var groups = groupFeed.GetValues();
            if (groups != null && !groups.Any(g => g.Contacts.IsNotNullOrEmpty()))
            {
                int loadCount = 0;
                foreach (var group in groups)
                {
                    var contactFeed = await sfClient.Groups.GetContacts(group.url).ExecuteAsync(ct);
                    group.NumberOfContacts = contactFeed.count;
                    group.Contacts = contactFeed.GetValues();
                    System.Diagnostics.Trace.TraceInformation("Group {0} of {1}: loaded {2} contacts for {3}",
                        ++loadCount, groupFeed.count, contactFeed.count, group.Name);
                }
            }
            return groupFeed;
        }
        #endregion

        public static Folder AsFolder(this Item item, bool isVerbose = false)
        {
            var folder = item as Folder;
            if (item is null)
                System.Diagnostics.Trace.TraceWarning("Item is null, empty folder returned");
            if (folder is null && string.IsNullOrEmpty(item.__type))
                System.Diagnostics.Trace.TraceWarning("Folder is null, empty folder returned");
            else if (folder is null)
                System.Diagnostics.Trace.TraceWarning("Item type is {0}, not a folder ({1})", item.__type, item.Id);
            else if (isVerbose)
                System.Diagnostics.Trace.TraceInformation("Folder loaded: {0}/{1}, containts {1}, {2}", folder.Name, folder.FileCount,
                    string.IsNullOrEmpty(folder.SemanticPath) ? folder.Id : folder.SemanticPath);
            return folder ?? new Folder();
        }

        public static Client.Models.File AsFile(this Item item)
        {
            var file = item as Client.Models.File;
            if (item is null || (file is null && string.IsNullOrEmpty(item.__type)))
                System.Diagnostics.Trace.TraceWarning("File is null");
            else if (file is null)
                System.Diagnostics.Trace.TraceWarning("Item type is {0}, not a file ({1})", item.__type, item.Id);
            else
                System.Diagnostics.Trace.TraceInformation("Folder loaded: '{0}' ({1})", file.Name, file.Id);
            return file;
        }

        public static async Task<Folder> GetDefaultFolderAndChildrenAsync(this IShareFileClient sfClient, CancellationToken ct = default)
        {
            var item = await sfClient?.Items?.Get().Expand("Children").ExecuteAsync(ct);
            return item.AsFolder();
        }

        public static async Task<Folder> GetRootFolderAccessControlsAsync(this IShareFileClient sfClient, CancellationToken ct = default)
        {
            var item = await sfClient.Items.Get().Expand("Parent").ExecuteAsync(ct);
            var root = await sfClient.Items.Get(item.Parent.url).Expand("AccessControls").Expand("Children").ExecuteAsync(ct);
            return root.AsFolder();
        }

        public static async Task<Folder> GetSharedFolderAccessControlsAsync(this IShareFileClient sfClient, CancellationToken ct = default)
        {
            var folderUri = sfClient.Items.GetAlias("allshared"); //top, 
            //var sharedFolder = await sfClient.Items.Get(allSharedUri).Expand("Children").ExecuteAsync(ct);
            var item = await sfClient.Items.Get(folderUri).Expand("AccessControls").Expand("Children").ExecuteAsync(ct);
            var paths = await sfClient.Items.GetBreadcrumbs(folderUri).SelectAndExecute(f => f.Name, ct);
            item.SemanticPath = Path.Combine(paths.ToArray()).Replace('\\', '/');
            return item.AsFolder();
        }

        public static async Task<IEnumerable<Item>> GetAllSharedFoldersForSpecificUserAsync(this IShareFileClient sfClient, string userUid, CancellationToken ct = default)
        {
            ODataFeed<Item> items = null;
            if (sfClient != null && !string.IsNullOrWhiteSpace(userUid))
            {
                var uri = sfClient.Users.GetEntityUriFromId(userUid);
                items = await sfClient.Users.GetAllSharedFoldersForSpecificUser(uri).ExecuteAsync(ct);
            }
            return items.GetValues();
        }

        public static async Task<Folder> GetTreeViewAsync(this IShareFileClient sfClient, string itemUid, string sourceUid, TreeMode treeMode = TreeMode.Standard, bool canCreateRootFolder = false, bool fileBox = false, string expandProperty = "Children", string selectProperty = "", CancellationToken ct = default)
        {
            var itemUri = sfClient.Items.GetAlias(itemUid);
            var itemQuery = sfClient.Items.Get(itemUri, treeMode, sourceUid, canCreateRootFolder, fileBox);
            if (!string.IsNullOrEmpty(expandProperty))
                itemQuery = itemQuery.Expand(expandProperty);
            if (!string.IsNullOrEmpty(selectProperty))
                itemQuery = itemQuery.Select(selectProperty);
            var item = await itemQuery.ExecuteAsync(ct);
            return item.AsFolder();
        }

        //public static async Task<IEnumerable<T>> GetJsonReportDataAsync<T>(this IShareFileClient sfClient, string recordUid) where T : class
        //{
        //    var reportJsonText = await sfClient.Reports.GetJsonData(recordUid).ExecuteAsync();
        //    return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<T>>(reportJsonText);
        //    //return await JsonSerializer.DeserializeAsync<IEnumerable<T>>(reportJsonText, _jsonOptions);
        //}

        public static async Task<IEnumerable<Report>> GetRecentReportsAsync(this IShareFileClient sfClient, int count = 10)
        {
            var recentReports = await sfClient.Reports.GetRecent(count).ExecuteAsync();
            var reports = recentReports.GetValues();
            return reports;
        }

        public static async Task<ReportRecord> GetReportRecordAsync(this IShareFileClient sfClient, string reportUid)
        {
            var uri = sfClient.Reports.GetEntityUriFromId(reportUid);
            var query = sfClient.Reports.GetRun(uri);
            var result = await query.ExecuteAsync();
            return result;
        }

        internal static async Task<Item> GetItemFromUidAsync(this IShareFileClient sfClient, string itemUid, bool getChildren = false, string sfPath = "")
        {
            if (sfClient is null || sfClient.Items is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (string.IsNullOrWhiteSpace(itemUid))
                throw new ArgumentNullException(nameof(itemUid));

            var itemUri = sfClient.Items.GetEntityUriFromId(itemUid); //same as GetAlias(itemUid);
            var itemQuery = sfClient.Items.Get(itemUri);

            if (getChildren)
                itemQuery = itemQuery.Expand("Children");

            var item = await itemQuery.ExecuteAsync();
            if (!string.IsNullOrWhiteSpace(sfPath))
                item.SemanticPath = sfPath;
            else
                item.SemanticPath = itemUri.AbsoluteUri;

            if (!getChildren) // Name is same as FileName //System.Net.WebUtility.UrlDecode(item.Name);
                System.Diagnostics.Trace.TraceInformation("Item loaded: '{0}' ({1})", item.Name, itemUid);

            return item;
        }

        public static async Task<Folder> GetFolderFromUidAsync(this IShareFileClient sfClient, string folderUid, params string[] sfPaths)
        {
            var sfPath = Path.Combine(sfPaths).Replace('\\', '/');
            var item = await sfClient.GetItemFromUidAsync(folderUid, getChildren: true, sfPath: sfPath);
            return item.AsFolder();
        }

        public static async Task<Client.Models.File> GetFileAsync(this IShareFileClient sfClient, string itemUid, params string[] sfPaths)
        {
            var sfPath = Path.Combine(sfPaths).Replace('\\', '/');
            var item = await sfClient.GetItemFromUidAsync(itemUid, getChildren: false, sfPath: sfPath);
            var file = item as Client.Models.File;
            if (file is null)
                System.Diagnostics.Trace.TraceWarning("Item type is {0}, not a file {1}", item.__type, itemUid);
            return file;
        }

        public static async Task<Item> GetFileAsync(this IShareFileClient sfClient, Item folderItem, CancellationToken ct = default)
        {
            if (sfClient is null || sfClient.Items is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (folderItem is null)
                throw new ArgumentNullException(nameof(folderItem));

            var itemUri = sfClient.Items.GetAlias(folderItem.Id);
            var item = await sfClient.Items.Get(itemUri).ExecuteAsync(ct);
            System.Diagnostics.Trace.TraceInformation("File loaded: '{0}' {1} ({2})",
                folderItem.SemanticPath, itemUri, folderItem.Id);

            return item;
        }

        public static async Task<IList<Item>> GetAllFilesAsync(this IShareFileClient sfClient, IEnumerable<Item> folderItems, CancellationToken ct = default)
        {
            IList<Item> items = new List<Item>();
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (folderItems != null)
                foreach (var item in folderItems)
                    if (!ct.IsCancellationRequested)
                        items.Add(await sfClient.GetFileAsync(item));
            return items;
        }

        internal static async Task<Item> GetItemFromPathAsync(this IShareFileClient sfClient, params string[] sfPaths)
        {
            if (sfClient is null || sfClient.Items is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (sfPaths is null || sfPaths.Length < 1 || string.IsNullOrWhiteSpace(sfPaths[0]))
                throw new ArgumentNullException(nameof(sfPaths));
            else if (sfPaths.Length == 1)
                sfPaths = sfPaths[0].Split('/');

            var sfPath = Path.Combine(sfPaths).Replace('\\', '/');
            var item = await sfClient.Items.ByPath(sfPath).ExecuteAsync();
            item.SemanticPath = sfPath;
            //System.Diagnostics.Trace.TraceInformation("Item loaded: '{0}' ({1})", sfPath, item.Id);

            return item;
        }

        public static async Task<Folder> GetFolderAsync(this IShareFileClient sfClient, params string[] sfPaths)
        {
            var item = await sfClient.GetItemFromPathAsync(sfPaths);
            var folder = item.AsFolder();
            System.Diagnostics.Trace.TraceInformation("Folder loaded: '{0}'", folder.SemanticPath);
            return folder;
        }

        public static async Task<Folder> GetFolderAndContentsAsync(this IShareFileClient sfClient, params string[] sfPaths)
        {
            var rootItem = await sfClient.GetItemFromPathAsync(sfPaths);
            var item = await sfClient.GetItemFromUidAsync(rootItem.Id, getChildren: true, sfPath: rootItem.SemanticPath);
            var folder = item.AsFolder();
            int number = folder.Children.Count();
            string fileNames = String.Join(", ", folder.Children.Select(o => o.Name));
            string subfolders = String.Format("{0} item(s): {1}", number, fileNames);
            System.Diagnostics.Trace.TraceInformation("Folder loaded: '{0}' ({1})", folder.SemanticPath, subfolders);
            return folder;
        }

        public static async Task<Folder> GetOrCreateFolderAsync(this IShareFileClient sfClient, Folder parentFolder, string folderName)
        {
            string parentPath = parentFolder.SemanticPath;
            if (parentFolder is null || parentFolder.url is null)
                throw new ArgumentNullException(nameof(parentFolder));
            else if (string.IsNullOrWhiteSpace(parentPath))
                throw new ArgumentNullException(nameof(parentFolder.SemanticPath));

            Folder folder;
            try
            {
                parentPath = Path.Combine(parentPath, folderName).Replace('\\', '/');
                var item = await sfClient.GetItemFromPathAsync(parentPath);
                folder = item.AsFolder();
            }
            catch (ODataException) //Folder not found
            {
                folder = await sfClient.CreateFolderAsync(parentFolder, folderName);
            }
            folder.SemanticPath = parentPath;

            return folder;
        }

        public static async Task<Folder> CreateFolderAsync(this IShareFileClient sfClient, Folder parentFolder, string folderName, string description = "", bool overwrite = true)
        {
            if (sfClient is null || sfClient.Items is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (parentFolder is null)
                throw new ArgumentNullException(nameof(parentFolder));
            else if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentNullException(nameof(folderName));

            // Create instance of the new folder we want to create.
            // Only a few properties on folder can be defined, others will be ignored.
            var newFolder = new Folder
            {
                Name = folderName,
                Description = description ?? ""
            };

            var folder = await sfClient.Items.CreateFolder(parentFolder.url, newFolder, overwrite: overwrite).ExecuteAsync();
            System.Diagnostics.Trace.TraceInformation("Folder created: '{0}', {1}", folderName, description);

            //Folder folder;
            //lock (GetLock(folderName)) //TODO test
            //{
            //    folder = sfClient.Items.CreateFolder(parentFolder.url, newFolder, overwrite: overwrite).Execute();
            //    System.Diagnostics.Trace.TraceInformation("Folder created: '{0}', {1}", folderName, description);
            //}
            //ReleaseLock(folderName);

            return folder;
        }

        public static async Task<string> UploadAsync(this IShareFileClient sfClient, Folder destinationFolder, string itemName)
        {
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (destinationFolder is null)
                throw new ArgumentNullException(nameof(destinationFolder));
            else if (string.IsNullOrWhiteSpace(itemName))
                throw new ArgumentNullException(nameof(itemName));

            var file = System.IO.File.Open(itemName, FileMode.OpenOrCreate);
            var uploadRequest = new UploadSpecificationRequest
            {
                FileName = itemName,
                FileSize = file.Length,
                Details = string.Empty,
                Parent = destinationFolder.url
            };

            var uploader = sfClient.GetAsyncFileUploader(uploadRequest, file);

            var uploadResponse = await uploader.UploadAsync();
            System.Diagnostics.Trace.TraceInformation("Folder uploaded: '{0}' ({1})", itemName, destinationFolder.url);

            return uploadResponse.First().Id;
        }

        public static async Task DownloadAsync(this IShareFileClient sfClient, Item itemToDownload, string uncFolderPath, CancellationToken ct = default)
        {
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (string.IsNullOrWhiteSpace(uncFolderPath))
                throw new ArgumentNullException(nameof(uncFolderPath), "No download path");
            else if (itemToDownload is null)
                return; //throw new ArgumentNullException(nameof(itemToDownload));

            var downloadDirectory = new DirectoryInfo(uncFolderPath);
            if (!downloadDirectory.Exists)
                downloadDirectory.Create();

            var downloader = sfClient.GetAsyncFileDownloader(itemToDownload);
            var filePath = Path.Combine(downloadDirectory.FullName, itemToDownload.Name);
            var file = System.IO.File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            System.Diagnostics.Trace.TraceInformation("Item downloaded to '{0}'", filePath);

            await downloader.DownloadToAsync(file, ct);
        }

        public static async Task DownloadAsync(this IShareFileClient sfClient, DownloadSpecification downloadSpecification, string uncFolderPath, string fileName = "", CancellationToken ct = default)
        {
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (string.IsNullOrWhiteSpace(uncFolderPath))
                throw new ArgumentNullException(nameof(uncFolderPath), "No download path");
            else if (downloadSpecification is null)
                return; //throw new ArgumentNullException(nameof(itemToDownload));

            var downloadDirectory = new DirectoryInfo(uncFolderPath);
            if (!downloadDirectory.Exists)
                downloadDirectory.Create();

            var downloader = sfClient.GetAsyncFileDownloader(downloadSpecification);
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = downloadSpecification.Id;
            var filePath = Path.Combine(downloadDirectory.FullName, fileName);
            var file = System.IO.File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            System.Diagnostics.Trace.TraceInformation("{0} downloaded to '{0}'", downloadSpecification.Id, filePath);

            await downloader.DownloadToAsync(file, ct);
        }

        public static async Task DownloadAsync(this IShareFileClient sfClient, Stream inStream, string uncFolderPath, string fileName, CancellationToken ct = default)
        {
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (inStream is null)
                throw new ArgumentNullException(nameof(inStream));
            else if (string.IsNullOrWhiteSpace(uncFolderPath))
                throw new ArgumentNullException(nameof(uncFolderPath));
            else if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            const int DefaultBufferSize = 81920;
            try
            {
                var downloadDirectory = new DirectoryInfo(uncFolderPath);
                if (!downloadDirectory.Exists)
                    downloadDirectory.Create();

                var filePath = Path.Combine(downloadDirectory.FullName, fileName);
                using (FileStream outStream = System.IO.File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    //if (inStream is not HttpBaseStream)
                    //inStream.Position = 0;
                    //inStream.Seek(0, SeekOrigin.Begin); // not supported by HttpBaseStream
                    //outStream.Seek(0, SeekOrigin.End); // not supported by HttpBaseStream
                    await inStream.CopyToAsync(outStream, DefaultBufferSize, ct);
                }
                System.Diagnostics.Trace.TraceInformation("Downloaded to '{0}'", filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Failed to download '{0}'. {1}: {2}", fileName, ex.GetType().Name, ex.Message);
                throw;
            }
        }

        public static async Task DownloadFolderAsync(this IShareFileClient sfClient, Folder folder, string uncFolderPath, CancellationToken ct = default)
        {
            if (folder is null)
                throw new ArgumentNullException(nameof(folder));

            string fileName = folder.Name + ".zip";
            var stream = await sfClient.GetFolderAsZipAsync(folder.Children, ct);
            await sfClient.DownloadAsync(stream, uncFolderPath, fileName, ct);
        }

        public static async Task<MemoryStream> GetStreamAsync(this IShareFileClient sfClient, Item itemToDownload)
        {
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (itemToDownload is null)
                throw new ArgumentNullException(nameof(itemToDownload));

            var outputStream = new MemoryStream();
            if (itemToDownload != null)
            {
                var downloader = sfClient.GetAsyncFileDownloader(itemToDownload);
                await downloader.DownloadToAsync(outputStream);
                outputStream.Position = 0;
            }
            return outputStream;
        }

        public static async Task<MemoryStream> GetFolderAsZipAsync(this IShareFileClient sfClient, IEnumerable<Item> items, CancellationToken ct = default)
        {
            var outputStream = new MemoryStream();

            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (items is null || !items.Any())
                return outputStream;

            using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, true))
            {
                var streamingTasks = items.Select(item => (item.FileName, GetStreamAsync(sfClient, item)));
                foreach (var task in streamingTasks)
                {
                    string fileName = task.FileName; //WebUtility.UrlDecode(task.FileName);
                    var fileInArchive = archive.CreateEntry(fileName);
                    System.Diagnostics.Trace.TraceInformation("'{0}' added to zip file.", fileName);
                    using (var entryStream = fileInArchive.Open())
                    {
                        var inputStream = await task.Item2;
                        inputStream.CopyTo(entryStream);
                    }
                }
            }

            outputStream.Position = 0;
            return outputStream;
        }

        public static async Task<Share> ShareViaLinkAsync(this IShareFileClient sfClient, Item fileToShare)
        {
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (fileToShare is null)
                throw new ArgumentNullException(nameof(fileToShare));

            var share = new Share
            {
                Items = new List<Item>
                {
                    fileToShare
                }
            };

            var shared = await sfClient.Shares.Create(share).ExecuteAsync();
            System.Diagnostics.Trace.TraceInformation("Item shared: '{0}' ({1})", fileToShare.Name, shared.url);

            return shared;
        }

        public static async Task ShareViaShareFileEmailAsync(this IShareFileClient sfClient, Item fileToShare, string recipientEmailAddress, string subject)
        {
            if (sfClient is null)
                throw new ArgumentNullException(nameof(sfClient));
            else if (fileToShare is null)
                throw new ArgumentNullException(nameof(fileToShare));
            else if (string.IsNullOrWhiteSpace(recipientEmailAddress))
                throw new ArgumentNullException(nameof(recipientEmailAddress));

            var sendShare = new ShareSendParams
            {
                Emails = new[] { recipientEmailAddress },
                Items = new[] { fileToShare.Id },
                Subject = subject ?? "",
                MaxDownloads = -1, // Allow unlimited downloads
                ExpirationDays = 10 // Expires in 10 days
            };

            await sfClient.Shares.CreateSend(sendShare).ExecuteAsync();
            System.Diagnostics.Trace.TraceInformation("Email for item '{0}' sent to '{1}'", fileToShare.Name, recipientEmailAddress);
        }
    }

    #region ShareFile objects
    public struct ShareFileItem
    {
        public Item Item { get; set; }
        public IEnumerable<ShareFileItem> Children { get; set; }
        public override string ToString() => $"{Item?.Name}";
    }
    #endregion
}