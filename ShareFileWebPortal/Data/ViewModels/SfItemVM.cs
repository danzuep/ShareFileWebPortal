using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShareFileWebPortal.Data.ViewModels
{
	/// <summary>
	/// Represents a ShareFile Item: an element that can exist inside a ShareFile Folder.
	/// This include Files and Folders, as well as other classes that are listed inside
	/// directory structures: Links, Notes and Symbolic Links.
	/// </summary>
	public record SfItemVM
	{
		/// <summary>
		/// Unique record ID
		/// </summary>
		[MaxLength(36)]
		public string Uid { get; set; }
		[MaxLength(36)]
		public string PrincipalUid { get; set; }
        [StringLength(100)]
		public string PrincipalName { get; set; }

		public string AccessType { get; set; }
		public string AccessName { get; set; }

		public string FullPath => System.IO.Path.Combine(SemanticPath ?? "", Name ?? "").Replace('\\', '/');

		private const int KiB = 1024;
		private const int SizeLimit = KiB * 10;

		public string FileSize
		{
			get
			{
				string size = "0 B";
				if (FileSizeBytes > 0 && FileSizeBytes < KiB)
					size = $"{FileSizeBytes} B";
				else if (FileSizeInKB == null)
					size = "?";
				else if (FileSizeInKB < SizeLimit)
					size = $"{FileSizeInKB} KB";
				else if (FileSizeInMB < SizeLimit)
					size = $"{FileSizeInMB} MB";
				else if (FileSizeInGB < SizeLimit)
					size = $"{FileSizeInGB} GB";
				return size;
			}
		}
		public int FileSizeInMB => FileSizeInKB.GetValueOrDefault() / KiB;
		public int FileSizeInGB => FileSizeInMB / KiB;

		/// <summary>
		/// Parent container of the Item. A container is usually a Folder object, with a few exceptions -
		/// the "Account" is the container of top-level folders.
		/// </summary>
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
		public long? FileSizeBytes { get; set; }

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
		/// Number of Items defined under this Folder, including sub-folder counts.
		/// </summary>
		public int? ItemCount { get; set; }

		public ICollection<ShareFileGroupVM> Groups { get; set; } = Array.Empty<ShareFileGroupVM>();

		public override string ToString() => String.Format("{0} {1} {2}", Uid, Name, SemanticPath);
	}
}