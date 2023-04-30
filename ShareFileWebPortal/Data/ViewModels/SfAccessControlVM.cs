using System;
using System.ComponentModel.DataAnnotations;

namespace ShareFileWebPortal.Data.ViewModels
{
	/// <summary>
	/// Represents a rule configuring access of a Principal to an Item.
	/// </summary>
	public record SfAccessControlVM
	{
		/// <summary>
		/// Unique record ID
		/// </summary>
		[MaxLength(36)]
		public string Id { get; set; }

		/// <summary>
		/// Item that was given permission through this rule
		/// </summary>
		//[ForeignKey("ItemId")]
		//public virtual SfItem Item { get; set; }
		//[ForeignKey("Item")]
		public string ItemId { get; set; }
		//public string? ItemPath { get; set; }
		//public string? ItemName { get; set; }
		/// <summary>
		/// Principal - User or Group - that has been granted permissions through this rule
		/// </summary>
		//[ForeignKey("PrincipalId")]
		//public virtual SfPrincipal Principal { get; set; }
		//[ForeignKey("Principal")]
		public string PrincipalId { get; set; }
		//public virtual SfUser User { get; set; }
		//public virtual SfGroup Group { get; set; }
		/// <summary>
		/// Defines whether the principal can add files (upload) into the Item
		/// </summary>
		public bool? CanUpload { get; set; }
		/// <summary>
		/// Defines whether the principal can read file content (download) from this Item
		/// </summary>
		public bool? CanDownload { get; set; }
		/// <summary>
		/// Defines whether the principal can view items (browse) from this Item
		/// </summary>
		public bool? CanView { get; set; }
		/// <summary>
		/// Defines whether the principal can remove items from this Item
		/// </summary>
		public bool? CanDelete { get; set; }
		/// <summary>
		/// Defines whether the principal can configure Access Controls in this Item
		/// </summary>
		public bool? CanManagePermissions { get; set; }
		/// <summary>
		/// Defines the notification preference for upload events. If set, the principal will receive
		/// notifications when new files are uploaded into this Item
		/// </summary>
		public bool? NotifyOnUpload { get; set; }
		/// <summary>
		/// Defines the notification preference for download events. If set, the principal will receive
		/// notifiation when items are downloaded from this Item.
		/// </summary>
		public bool? NotifyOnDownload { get; set; }
		/// <summary>
		/// Defines whether the principal is the owner of this Item
		/// </summary>
		public bool? IsOwner { get; set; }
	}
}