using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.ShareFile.Models
{
	/// <summary>
	/// Distribution Group
	/// </summary>
	//[Table("Groups")]
	public record SfGroup : SfPrincipal
	{
		public override int Discriminator { get; set; } = (int)SfPrincipalType.Group;

		/// <summary>
		/// The group's owner
		/// </summary>
		//[ForeignKey("OwnerId")]
		//public virtual SfPrincipal Owner { get; set; }
		//public string OwnerId { get; set; }
		/// <summary>
		/// Whether this group is public
		/// </summary>
		public bool? IsShared { get; set; }
		/// <summary>
		/// Number of group contacts
		/// </summary>
		//[Column("NumberOfContacts")]
		public int? MemberCount { get; set; }
		/// <summary>
		/// List of group contacts
		/// </summary>
		public virtual ICollection<SfPrincipal> Members { get; set; }
	}
}