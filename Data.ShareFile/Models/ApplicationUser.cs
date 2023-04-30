using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.ShareFile.Models
{
    public partial class ApplicationUser
    {
        public ApplicationUser()
        {
            ShareFileGroupCreatedByNavigations = new HashSet<ShareFileDbGroup>();
            ShareFileGroupMemberCreatedByNavigations = new HashSet<ShareFileDbGroupMember>();
            ShareFileGroupMemberModifiedByNavigations = new HashSet<ShareFileDbGroupMember>();
            ShareFileGroupModifiedByNavigations = new HashSet<ShareFileDbGroup>();
            CompanyDefaultApprover2s = new HashSet<Company>();
            CompanyDefaultApprover3s = new HashSet<Company>();
            CompanyDefaultApprovers = new HashSet<Company>();
            CompanyDefaultPreparers = new HashSet<Company>();
            CompanyDefaultReviewers = new HashSet<Company>();
            SecurityUserCompanyApplicationUsers = new HashSet<SecurityUserCompany>();
            SecurityUserCompanyCreatedBies = new HashSet<SecurityUserCompany>();
            SecurityUserCompanyModifiedBies = new HashSet<SecurityUserCompany>();
            SharefileAuthCreatedBies = new HashSet<SharefileAuth>();
            SharefileAuthModifiedBies = new HashSet<SharefileAuth>();
        }

        [Key]
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        public bool Enabled { get; set; }
        [Required(ErrorMessage = "An AD login is required")]
        public string AdLogin { get; set; }
        [Required(ErrorMessage = "A first name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "A last name is required")]
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public bool InternalReviewer { get; set; }

        public virtual ICollection<ShareFileDbGroup> ShareFileGroupCreatedByNavigations { get; set; }
        public virtual ICollection<ShareFileDbGroupMember> ShareFileGroupMemberCreatedByNavigations { get; set; }
        public virtual ICollection<ShareFileDbGroupMember> ShareFileGroupMemberModifiedByNavigations { get; set; }
        public virtual ICollection<ShareFileDbGroup> ShareFileGroupModifiedByNavigations { get; set; }
        public virtual ICollection<Company> CompanyDefaultApprover2s { get; set; }
        public virtual ICollection<Company> CompanyDefaultApprover3s { get; set; }
        public virtual ICollection<Company> CompanyDefaultApprovers { get; set; }
        public virtual ICollection<Company> CompanyDefaultPreparers { get; set; }
        public virtual ICollection<Company> CompanyDefaultReviewers { get; set; }
        public virtual ICollection<SecurityUserCompany> SecurityUserCompanyApplicationUsers { get; set; }
        public virtual ICollection<SecurityUserCompany> SecurityUserCompanyCreatedBies { get; set; }
        public virtual ICollection<SecurityUserCompany> SecurityUserCompanyModifiedBies { get; set; }
        public virtual ICollection<SharefileAuth> SharefileAuthCreatedBies { get; set; }
        public virtual ICollection<SharefileAuth> SharefileAuthModifiedBies { get; set; }
    }
}
