using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.ShareFile.Models
{
	/// <summary>
	/// Represents a ShareFile Item: an element that can exist inside a ShareFile Folder.
	/// This include Files and Folders, as well as other classes that are listed inside
	/// directory structures: Links, Notes and Symbolic Links.
	/// </summary>
	public record SfItem : EntityUidBase
	{
		/// <summary>
		/// Parent container of the Item. A container is usually a Folder object, with a few exceptions -
		/// the "Account" is the container of top-level folders.
		/// </summary>
		//[ForeignKey("ParentId")]
		//public virtual SfItem Parent { get; set; }
		public string ParentId { get; set; }
		/// <summary>
		/// Item Name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Item File Name. ShareFile allows Items to have different Display and File names: display
		/// names are shown during client navigation, while file names are used when the item is
		/// downloaded.
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// Item description
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Item size in Kilobytes. For containers, this field includes all children sizes, recursively.
		/// </summary>
		public int? FileSizeInKB { get; set; }
		/// <summary>
		/// Item size in bytes. For containers, this field will include all children sizes, recursively.
		/// </summary>
		[NotMapped]
		public long? FileSizeBytes { get; set; }
		/// <summary>
		/// Defines whether the Item has a 'hidden' flag.
		/// </summary>
		public bool? IsHidden { get; set; }
		public int? State { get; set; }
		/// <summary>
		/// Contains a ItemID path, separated by /, from the virtual root to this given file. Example
		/// /accountID/folderID/folderID/itemID
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// Item Path using Folder names
		/// </summary>
		public string SemanticPath { get; set; }
		/// <summary>
		/// Specifies whether there are other versions of this item. Not all providers support file versioning. The
		/// Capability FileVersioning indicates whether the provider supports file versions.
		/// </summary>
		public bool? HasMultipleVersions { get; set; }
		/// <summary>
		/// MD5 Hash of the File contents.
		/// </summary>
		public string Hash { get; set; }
		/// <summary>
		/// List of Access Controls for this Item. This is not the effective ACL on the Item, just the
		/// ACLs directly attached to this Item. Use the "Info" reference to retrieve effective ACL
		/// </summary>
		//public virtual ICollection<SfAccessControl> AccessControls { get; set; }
		/// <summary>
		/// Number of Items defined under this Folder, including sub-folder counts.
		/// </summary>
		[Column("ItemCount")]
		public int? ItemCount { get; set; }
		/// <summary>
		/// List of Children defined under this folder.
		/// </summary>
		//public virtual ICollection<SfItem> Children { get; set; }
	}
}