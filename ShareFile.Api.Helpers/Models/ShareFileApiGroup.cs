using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShareFile.Api.Helpers.Models
{
    //https://api.sharefile.com/docs/resource?name=ShareFile.Api.Models.Group
    public class ShareFileApiGroup
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public bool IsShared { get; set; }
        public string Url { get; set; }

        public override string ToString() => $"Group({Uid}) {Name}, IsShared={IsShared}.";
    }

    public class ShareFileGroupChildJson : OData
    {
        public bool IsShared { get; set; }
        public string Name { get; set; }

        public override string ToString() => $"{Type}({Uid}) {Name}, IsShared={IsShared}.";
    }

    public static class ShareFileGroupExtenions
    {
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/
        public static readonly Expression<Func<ShareFileGroupChildJson, ShareFileApiGroup>> AsGroupDto =
            group => new ShareFileApiGroup
            {
                Uid = group.Uid,
                Name = group.Name,
                Url = group.Url,
                IsShared = group.IsShared
            };

        public static IQueryable<ShareFileApiGroup> GetGroups(IQueryable<ShareFileGroupChildJson> groups)
        {
            return groups.Select(AsGroupDto);
        }

        public static ShareFileApiGroup Convert(
            this ShareFileGroupChildJson group, string rootUrl = "")
        {
            return new ShareFileApiGroup()
            {
                Uid = group.Uid,
                Name = group.Name,
                Url = group.Url,
                IsShared = group.IsShared
            };
        }
    }
}