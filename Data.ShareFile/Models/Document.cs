using System;
using System.Collections.Generic;

namespace Data.ShareFile.Models
{
    public partial class Document
    {
        public int Id { get; set; }
        public string RootPath { get; set; }
        public string FolderPath { get; set; }
        public string FileName { get; set; }
        public byte[] RawData { get; set; }
        public int? UploadedById { get; set; }
        public DateTime? UploadedOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
