using System;
using System.Collections.Generic;

namespace Data.ShareFile.Models
{
    public partial class ViewShareFileGroupMember
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Groups { get; set; }
        public string Company { get; set; }
        public string Uid { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool? IsDisabled { get; set; }
        public DateTime? LastAnyLogin { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
