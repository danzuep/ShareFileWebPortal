using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareFile.Api.Helpers.Models
{
    //https://docs.microsoft.com/en-us/odata/client/code-generation-tool
    public class ODataRoot<IOData> : OData
    {
        [JsonPropertyName("odata.count")]
        public int Count { get; set; }

        [JsonPropertyName("value")]
        public IList<IOData> Values { get; set; }

        public override string ToString() => $"{Url} ({Count}) has {Values?.Count ?? 0} members, {Metadata}."; //Values.ToEnumeratedString()
    }

    public class OData : IOData
    {
        //[Key]
        //[JsonIgnore]
        //[Display(Name = "ID")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        [JsonPropertyName("odata.metadata")]
        public string Metadata { get; set; }

        [NotMapped]
        [Display(Name = "OData Type")]
        [JsonPropertyName("odata.type")]
        public string Type { get; set; }

        [Display(Name = "UID")]
        [JsonPropertyName("Id")]
        public string Uid { get; set; }

        [NotMapped]
        [Display(Name = "URL")]
        [JsonPropertyName("url")]
        public string Url { get; set; }

        public override string ToString() => $"{Uid} <{Type}> {Url}";
    }

    public interface IOData
    {
        string Metadata { get; set; }
        string Type { get; set; }
        string Uid { get; set; }
        string Url { get; set; }
    }

    public static class ODataExtensions
    {
        public static IEnumerable<TOut> GetValues<TIn, TOut>(this ODataRoot<TIn> data, Func<TIn, TOut> asDto) where TIn : IOData
        {
            return data.Values.Select(asDto);
        }
    }
}
