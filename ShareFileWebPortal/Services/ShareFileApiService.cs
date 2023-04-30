using System.Diagnostics;
using ShareFile.Api.Client;
using ShareFile.Api.Client.Models;
using ShareFile.Api.Helpers;
using ShareFile.Api.Helpers.Models;
using ShareFile.Api.Helpers.Extensions;
using AutoMapper;
using Common.Helpers;
using Data.ShareFile.Models;
using ShareFileWebPortal.Data.ViewModels;
using ShareFileWebPortal.Models;
using Microsoft.Extensions.Options;

namespace ShareFileWebPortal.Services
{
    public class ShareFileApiService
    {
        private static string _name = nameof(ShareFileApiService);
        private readonly ILogger _logger = LogProvider.GetLogger(_name);
        private readonly ExtendedShareFileApiOptions? _sfOptions;
        private string _sfAccessToken = string.Empty;
        private static ShareFileGroupVM? _sfGroup;
        //private readonly ShareFileRepository _db;
        private readonly Task _initialisation;
        private readonly IMapper _mapper;
        private HttpClient _httpClient;
        private IShareFileClient _sfClient;

        public ShareFileApiService(ILogger<ShareFileApiService> logger, IMapper mapper, IOptions<AppsettingsOptions> options)
        {
            _logger = logger;
            _mapper = mapper;
            var appOptions = options.Value;
            _sfOptions = appOptions.ShareFile;
            _logger.LogInformation("{0} started at {1}", _name, DateTimeOffset.Now.ToDateTimeString());
            //_db = db;
            _initialisation = InitialiseAsync();
        }

        private async Task InitialiseAsync(CancellationToken ct = default)
        {
            if (_sfOptions is null)
                System.Diagnostics.Debugger.Break();
            if (string.IsNullOrEmpty(_sfAccessToken) && _sfOptions != null)
            {
                ShareFileApi.Logger = LogProvider.GetLogger(ShareFileApi.Name);
                var sfService = _sfOptions.InitialiseShareFileSevice;
                var sfToken = await sfService.StartSessionAsync(_sfOptions.Credential);
                _sfAccessToken = sfToken.AccessToken;
                _sfClient = sfService.ShareFileClient;
                //_httpClient = ShareFileApi.GetHttpClient(_sfOptions?.SubDomain, _sfAccessToken);
            }
        }

        public async Task<IList<ShareFileGroupVM>> GetAllGroupMembersAsync(CancellationToken ct = default)
        {
            IList<ShareFileGroupVM> groupsVM = new List<ShareFileGroupVM>();
            if (await GetGroupsAsync(ct) is IEnumerable<ShareFileGroupVM> groups)
            {
                foreach (var groupVM in groups)
                {
                    if (ct.IsCancellationRequested)
                        break;
                    if (groupVM is ShareFileGroupVM group)
                    {
                        var memberVMs = await GetGroupMembersAsync(group, ct);
                        group.Members = memberVMs.ToList();
                        groupsVM.Add(group);
                    }
                }
            }
            return groupsVM;
        }

        public async Task<IEnumerable<ShareFileGroupVM>> GetGroupsAsync(CancellationToken ct = default)
        {
            await _initialisation;
            IEnumerable<ShareFileGroupVM> groupsVM = Array.Empty<ShareFileGroupVM>();
            if (await GetShareFileGroupsAsync(ct) is IEnumerable<ShareFileDbGroup> groups)
                groupsVM = groups.Select(m => m.ConvertVM());
            _logger.LogInformation("ShareFile has {1} groups.", groupsVM.Count());
            return groupsVM;
        }

        public async Task<IEnumerable<ShareFileDbGroup>> GetShareFileGroupsAsync(CancellationToken ct = default)
        {
            var groups = await GetShareFileApiGroupsAsync(ct);
            //var groupsViewModel = _mapper.Map<ShareFileDbGroup>(groups);
            return groups.Select(g => g.ConvertDB());
        }

        public async Task<IEnumerable<ShareFileApiGroup>> GetShareFileApiGroupsAsync(CancellationToken ct = default)
        {
            await _initialisation;
            using var httpClient = ShareFileApi.GetHttpClient(_sfOptions?.SubDomain, _sfAccessToken);
            return await httpClient.LoadShareFileGroups(ct);
        }

        public async Task<IEnumerable<ShareFileGroupMemberVM>> GetGroupMembersAsync(string groupUid, string groupName, CancellationToken ct = default)
        {
            var apiMembers = await GetShareFileApiGroupMembersAsync(groupUid, ct);
            var membersVM = apiMembers?.Select(m => m.ConvertVM(groupName));
            if (membersVM.IsNotNullOrEmpty())
                _logger.LogInformation("{0} group has {1} members.", groupName ?? groupUid, membersVM.Count());
            return membersVM != null ? membersVM : Array.Empty<ShareFileGroupMemberVM>();
        }

        public async Task<IEnumerable<ShareFileGroupMemberVM>> GetGroupMembersAsync(ShareFileGroupVM group, CancellationToken ct = default)
        {
            IEnumerable<ShareFileGroupMemberVM> membersVM = Array.Empty<ShareFileGroupMemberVM>();
            if (group is ShareFileGroupVM groupVM)
            {
                //var members = await GetShareFileGroupMembersAsync(groupVM.Uid, ct);
                var members = await GetShareFileApiGroupMembersAsync(groupVM.Uid, ct);
                membersVM = members.Select(m => m.ConvertVM(group.Name));
                _logger.LogInformation("{0} group has {1} members.", group.Name, membersVM.Count());
            }
            return membersVM;
        }

        public async Task<IEnumerable<ShareFileDbGroupMember>> GetShareFileGroupMembersAsync(ShareFileDbGroup group, CancellationToken ct = default)
        {
            var members = await GetShareFileApiGroupMembersAsync(group.Uid, ct);
            //var membersViewModel = _mapper.Map<ShareFileDbGroupMember>(members);
            return members?.Select(g => g.Convert(group)) ?? Array.Empty<ShareFileDbGroupMember>();
        }

        public async Task<IEnumerable<ShareFileApiGroupMember>> GetShareFileApiGroupMembersAsync(string groupUid, CancellationToken ct = default)
        {
            await _initialisation;
            using var httpClient = ShareFileApi.GetHttpClient(_sfOptions?.SubDomain, _sfAccessToken);
            return await httpClient.LoadShareFileGroupMembers(groupUid, ct);
        }

        #region Principals, Users, Contacts, Groups, Folders, Access
        public async Task<IEnumerable<Item>> GetAllSharedFoldersForSpecificUserAsync(string userUid, CancellationToken ct = default)
        {
            await _initialisation;
            return await _sfClient.GetAllSharedFoldersForSpecificUserAsync(userUid, ct);
        }

        public async Task<ODataFeed<Contact>> GetShareFileApiContactsAsync(CancellationToken ct = default)
        {
            await _initialisation;
            //var contacts = await _sfClient.Accounts.GetEmployees().ExecuteAsync();
            return await _sfClient.Accounts.GetAddressBook("shared").ExecuteAsync(ct);
        }

        public async Task<IList<Folder>> GetShareFileApiFoldersAsync(string folderToProcess, string? folderToSkip = null, int folderDepthLimit = -1, CancellationToken ct = default)
        {
            await _initialisation;
            var splitChars = new char[] { '|', '&', ',', ';', ' ' };
            var foldersToSkip = folderToSkip?.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            var folders = await _sfClient.GetFolderAccessControlsAsync(folderToProcess, folderDepthLimit, foldersToSkip, ct: ct);
            //var folderAccess = folders.SelectMany(f => f.AccessControls);
            return folders;
        }

        public async Task<IList<Folder>> GetChildrenAsync(Folder? folder, string? folderToSkip = null, CancellationToken ct = default)
        {
            await _initialisation;
            IList<Folder> folders = Array.Empty<Folder>();
            if (folder != null)
            {
                try
                {
                    var foldersToSkip = folderToSkip?.Split('|');
                    folders = await _sfClient.GetRecursiveFolderAccess(folder, foldersToSkip, ct: ct);
                }
                catch (ObjectDisposedException ex)
                {
                    string folderUid = folders.LastOrDefault()?.Id ?? "";
                    string subdomain = _sfClient?.BaseUri?.Host?.Split('.').FirstOrDefault() ?? "";
                    var url = string.Format("https://{0}.sharefile.com/home/shared/{1}", subdomain, folderUid);
                    _logger.LogError(ex, "ShareFile client disposed while attempting to access {0}", url);
                    await InitialiseAsync(ct); //var refreshedSfClient = await sfService?.RefreshSfClientAsync(sfAccessToken);
                    folder = await _sfClient.GetFolderAccessControlsAsync(folderUid, ct);
                    var childFolders = await GetChildrenAsync(folder, folderToSkip, ct);
                    folders = Enumerable.Concat(folders.SkipLast(1), childFolders).ToList();
                }
            }
            return folders ?? Array.Empty<Folder>();
        }

        public async Task<IList<Folder>> GetSharedFolderAndChildrenAsync(string? folderToSkip = null, CancellationToken ct = default)
        {
            await _initialisation;
            var folder = await _sfClient.GetSharedFolderAccessControlsAsync(ct);
            return await GetChildrenAsync(folder, folderToSkip, ct);
        }

        public async Task<IList<Folder>> GetRootFolderAndChildrenAsync(string? folderToSkip = null, CancellationToken ct = default)
        {
            var folder = await _sfClient.GetRootFolderAccessControlsAsync(ct);
            var foldersToSkip = folderToSkip?.Split('|');
            var folders = await _sfClient.GetRecursiveFolderAccess(folder, foldersToSkip, ct: ct);
            return folders;
        }
        #endregion
    }
}