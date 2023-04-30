using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShareFile.Api.Helpers.Models
{
    //https://api.sharefile.com/docs/resource?name=ShareFile.Api.Models.User
    public class ShareFileApiUser : OData
    {
        [JsonPropertyName("FullName")]
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullNameShort { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        public string MemberUid { get; set; }
        public string Company { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime? LastAnyLogin { get; set; }
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("FavoriteFolders")]
        public IList<IOData> FavoriteFolders { get; set; }

        [JsonPropertyName("Favorites")]
        public IList<IOData> Favorites { get; set; }

        [JsonPropertyName("Groups")]
        public IList<IOData> Groups { get; set; }

        public override string ToString() => $"{Type}({Uid}) {FullName}, Email={Email}.";
    }
}
