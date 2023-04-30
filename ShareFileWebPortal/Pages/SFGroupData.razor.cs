using Common.Helpers;
using Data.ShareFile;
using Data.ShareFile.Models;
using ShareFileWebPortal.Data.ViewModels;

namespace ShareFileWebPortal.Pages
{
    public partial class SFGroupData
    {
        private IEnumerable<ShareFileDbGroup>? _dbSfGroups;
        private ICollection<ShareFileGroupVM>? sfGroups;
        private int groupCount => sfGroups?.Count ?? 0;
        private string filter = ""; // "prefix";

        protected override async Task OnInitializedAsync()
        {
            await GetGroupsFromDb();
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

        private async Task UpdateDatabaseGroups()
        {
            _logger.LogInformation("Saving to the database...");
            int updateCount = await _db.SetRepoShareFileGroups(_dbSfGroups);
            _logger.LogInformation("Updated {0} group records", updateCount);
        }
    }
}