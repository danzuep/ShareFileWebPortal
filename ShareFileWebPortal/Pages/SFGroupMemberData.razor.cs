using Common.Helpers;
using Microsoft.AspNetCore.Components;
using ShareFileWebPortal.Services;
using ShareFileWebPortal.Data.ViewModels;
using Data.ShareFile.Models;
using Data.ShareFile;
using ShareFile.Api.Helpers.Models;

namespace ShareFileWebPortal.Pages
{
    public partial class SFGroupMemberData
    {
        [Parameter] public ShareFileDbGroup? selectedDbSfGroup { get; set; }
        [Parameter] public string filter { get; set; } = ""; // folder filter prefix
        [Inject] ShareFileApiService? _sfService { get; set; }
        [Inject] ShareFileRepository? _db { get; set; }

        private string initialText = "-- Group Selection --";
        private IEnumerable<ShareFileDbGroup>? _dbSfGroups;
        private ICollection<ShareFileGroupVM>? sfGroups;
        private ICollection<ShareFileGroupMemberVM>? SFGroupMembers;
        private int memberCount => SFGroupMembers?.Count ?? 0;
        private int groupCount => sfGroups?.Count ?? 0;

        protected override async Task OnInitializedAsync()
        {
            if (sfGroups is null)
            {
                await GetGroupsFromDb();
            }
        }

        private async Task GroupChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value as string, out int selectedId))
            {
                _logger.LogInformation($"Group {selectedId} selected");
                selectedDbSfGroup = _dbSfGroups?.FirstOrDefault(g => g.Id == selectedId);
                await GetGroupMembersFromDb();
            }
        }

        private async Task GetGroupsFromDb()
        {
            _logger.LogInformation("Loading groups from DB...");
            _dbSfGroups = string.IsNullOrWhiteSpace(filter) ?
                await _db.GetShareFileGroups() :
                await _db.GetFilteredShareFileGroups(filter);
            sfGroups = _dbSfGroups.ConvertVM().ToList();
            if (sfGroups != null)
                _logger.LogDebug(string.Join('\n', sfGroups.Select(a => $"{a.Id,3} {a.Name}")));
            _logger.LogInformation("{0} groups ready to view", groupCount);
        }

        private async Task GetGroupsFromApi()
        {
            _logger.LogInformation("Loading groups from API...");
            _dbSfGroups = await _sfService.GetShareFileGroupsAsync();
            sfGroups = _dbSfGroups.ConvertVM().ToList();
            if (sfGroups != null)
                _logger.LogDebug(string.Join('\n', sfGroups.Select(a => $"{a.Uid} {a.Name}")));
            _logger.LogInformation("{0} groups ready to view", groupCount);
            Task.Run(async () => await UpdateDatabaseGroups());
        }

        private async Task GetAllGroupMembersFromApi()
        {
            _logger.LogInformation("Loading groups...");
            CancellationToken ct = default; //TODO link to context
            var dbGroups = await _sfService.GetShareFileGroupsAsync(ct);
            _logger.LogInformation("{0} groups loaded", groupCount);
            if (dbGroups.IsNotNullOrEmpty())
            {
                var groups = dbGroups.ToList();
                for (int i = 0; i < groups.Count; i++)
                {
                    if (ct.IsCancellationRequested)
                        break;
                    _logger.LogInformation("Loading members for group {0} of {1}...", i + 1, groups.Count);
                    var dbMembers = await _sfService.GetShareFileGroupMembersAsync(groups[i], ct);
                    _logger.LogInformation("Group members loaded");
                    groups[i].ShareFileGroupMembers = dbMembers.ToList();
                    _logger.LogInformation("Saving {0} members to the database...", memberCount);
                    await _db.SetRepoShareFileGroup(groups[i], ct);
                    _logger.LogInformation("Updated {0} group member records", memberCount);
                }
            }
        }

        private async Task UpdateDatabaseGroups()
        {
            _logger.LogInformation("Saving to the database...");
            int updateCount = await _db.SetRepoShareFileGroups(_dbSfGroups);
            _logger.LogInformation("Updated {0} group records", updateCount);
        }

        private async Task GetGroupMembersFromDb()
        {
            if (_db != null && selectedDbSfGroup != null)
            {
                _logger.LogInformation("Loading group members from DB...");
                var dbSfGroupMembers = await _db.GetShareFileGroupMembers(selectedDbSfGroup.Id);
                SFGroupMembers = dbSfGroupMembers.ConvertVM().ToList();
                if (SFGroupMembers != null)
                    _logger.LogDebug(string.Join('\n', SFGroupMembers.Select(a => $"{a.Id,4} {a.FullName}")));
                _logger.LogInformation("{0} group members ready to view", memberCount);
            }
        }

        private async Task GetGroupMembersFromApi()
        {
            if (_sfService != null && selectedDbSfGroup != null)
            {
                _logger.LogInformation("Loading group {0} members from API...", selectedDbSfGroup.Id);
                var dbSfGroupMembers = await _sfService.GetShareFileGroupMembersAsync(selectedDbSfGroup);
                if (dbSfGroupMembers != null)
                {
                    selectedDbSfGroup.ShareFileGroupMembers = dbSfGroupMembers.ToList();
                    SFGroupMembers = dbSfGroupMembers.ConvertVM().ToList();
                    if (SFGroupMembers != null)
                        _logger.LogDebug(string.Join('\n', SFGroupMembers.Select(a => $"{a.MemberUid} {a.FullName}")));
                }
                _logger.LogInformation("{0} group members ready to view", memberCount);
                Task.Run(async () => await UpdateDatabaseGroupMembers());
            }
        }

        private async Task UpdateDatabaseGroupMembers()
        {
            if (_db != null && selectedDbSfGroup?.ShareFileGroupMembers != null)
            {
                _logger.LogInformation("Saving to the database...");
                int updateCount = await _db.SetRepoShareFileGroup(selectedDbSfGroup);
                //int updateCount = await _db.AddOverwriteAsync(selectedDbSfGroup);
                _logger.LogInformation("Updated {0} group member records", updateCount);
            }
        }
    }
}