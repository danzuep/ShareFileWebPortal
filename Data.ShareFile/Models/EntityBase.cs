using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.ShareFile.Models
{
    /// <summary>
    /// Base model class to handle generic table fields
    /// </summary>
    public abstract record EntityBase
    {
        [Display(Name = "Is Deleted")]
        [ScaffoldColumn(false)]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Created By")]
        [ForeignKey("CreatedByNavigation")]
        [ScaffoldColumn(false)]
        public int? CreatedBy { get; set; }

        [Display(Name = "Created On")]
        [Required( ErrorMessage = "A creation date is required")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Display(Name = "Modified By")]
        [ForeignKey("ModifiedByNavigation")]
        [ScaffoldColumn(false)]
        public int? ModifiedBy { get; set; }

        [Display(Name = "Modified On")]
        [Required(ErrorMessage = "A modification date is required")]
        [DataType(DataType.DateTime)]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        [NotMapped]
        [ScaffoldColumn(false)]
        public DateTime LoadedOn { get; } = DateTime.Now;

        public virtual ApplicationUser CreatedByNavigation { get; set; }

        public virtual ApplicationUser ModifiedByNavigation { get; set; }
    }
}
