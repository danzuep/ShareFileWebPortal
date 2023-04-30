using System;
using System.Collections.Generic;

namespace Data.ShareFile.Models
{
    public partial class SecurityCompanyPermission
    {
        public int Id { get; set; }
        public int PermissionId { get; set; }
        public string Member { get; set; }
        public string RoleId { get; set; }
        public int? ConsolCompId { get; set; }
        public string BusinessStream { get; set; }
        public int? CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
    }
}
