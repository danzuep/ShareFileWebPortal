using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShareFile.Api.Client.Requests;
using ShareFile.Api.Client.Models;
using ShareFile.Api.Client;
using ShareFile.Api.Client.Entities;

namespace ShareFile.Api.Helpers.Extensions
{
    public static class QueryExtensions
    {
        public static IEnumerable<T> GetValues<T>(this ODataFeed<T> data) where T : class
        {
            return data?.Feed ?? Array.Empty<T>();
        }

        public static IEnumerable<T> ExecuteFeed<T>(this IQuery<ODataFeed<T>> modelQuery)
            where T : ODataObject
        {
            return modelQuery.Execute().GetValues();
        }

        public static async Task<IEnumerable<T>> ExecuteFeedAsync<T>(this IQuery<ODataFeed<T>> modelQuery, CancellationToken ct = default)
            where T : ODataObject
        {
            var oDataFeed = await modelQuery.ExecuteAsync(ct);
            return oDataFeed.GetValues();
        }

        private static Uri GetUriFromId(this IEntityBase entity, string uniqueId, string type = "Items")
        {
            string originalType = entity.Entity;
            entity.Entity = type;
            var uri = entity.GetEntityUriFromId(uniqueId);
            entity.Entity = originalType;
            return uri;
        }

        /// <summary>
        /// Get List of User Shared Folders
        /// </summary>
        /// <remarks>
        /// Retrieve the list of shared folders the specified user has access to
        /// </remarks>
        /// <returns>
        /// A list of Folder objects, representing shared folders of an user
        /// </returns>
        public static IQuery<ODataFeed<Item>> GetAllSharedFoldersForSpecificUser(this IUsersEntity entity, Uri url, bool includeDeleted = false)
        {
            var sfApiQuery = new Query<ODataFeed<Item>>(entity.Client);
            sfApiQuery.Uri(url);
            sfApiQuery.Action("AllSharedFolders");
            sfApiQuery.QueryString("includeDeleted", includeDeleted);
            sfApiQuery.HttpMethod = "GET";
            return sfApiQuery;
        }

        public static IQuery<ODataFeed<Item>> GetAllSharedFoldersForSpecificUser(this IUsersEntity entity, string uniqueId)
        {
            var uri = entity.GetEntityUriFromId(uniqueId);
            //var uri = entity.GetUriFromId(uniqueId, "Items");
            return entity.GetAllSharedFoldersForSpecificUser(uri);
        }

        //public static IQuery<ODataFeed<Item>> GetAllSharedFoldersForSpecificUser(this IItemsEntity entity, Uri url)
        //{
        //    var sfApiQuery = new Query<ODataFeed<Item>>(entity.Client);
        //    sfApiQuery.Action("AllSharedFolders");
        //    sfApiQuery.Uri(url);
        //    sfApiQuery.HttpMethod = "GET";
        //    return sfApiQuery;
        //}

        public static IQuery<ODataFeed<AccessControl>> GetAccessControls(this IAccessControlsEntity entity, string uniqueId)
        {
            var uri = entity.GetUriFromId(uniqueId, "Items");
            return entity.GetByItem(uri);
        }

        public static IQuery<ODataFeed<AccessControl>> GetAccessControls(this IItemsEntity entity, string uniqueId)
        {
            var uri = entity.GetEntityUriFromId(uniqueId);
            var sfApiQuery = new Query<ODataFeed<AccessControl>>(entity.Client);
            sfApiQuery.Uri(uri);
            sfApiQuery.Action("AccessControls");
            sfApiQuery.HttpMethod = "GET";
            return sfApiQuery;
        }

        //public static IQuery<AccessControl> GetAccessControls(this IAccessControlsEntity access, string userUid, string folderUid)
        //{
        //    var sfApiQuery = new Query<AccessControl>(access.Client);
        //    sfApiQuery.From("AccessControls");
        //    sfApiQuery.QueryString("principalId", userUid);
        //    sfApiQuery.QueryString("itemId", folderUid);
        //    sfApiQuery.HttpMethod = "GET";
        //    return sfApiQuery;
        //}

        //public static IQuery<ODataFeed<AccessControl>> GetAccessControls(this IAccessControlsEntity access, string userUid, string folderUid)
        //{
        //    var sfApiQuery = new Query<ODataFeed<AccessControl>>(access.Client);
        //    sfApiQuery.From("AccessControls");
        //    sfApiQuery.QueryString("principalId", userUid);
        //    sfApiQuery.QueryString("itemId", folderUid);
        //    sfApiQuery.HttpMethod = "GET";
        //    return sfApiQuery;
        //}

        //public static IQuery<ODataFeed<Item>> GetAccessControls(this IGroupsEntity group)
        //{
        //    var sfApiQuery = new Query<ODataFeed<Item>>(group.Client);
        //    sfApiQuery.From("Groups");
        //    sfApiQuery.Action("AccessControls");
        //    sfApiQuery.HttpMethod = "GET";
        //    return sfApiQuery;
        //}
    }
}
