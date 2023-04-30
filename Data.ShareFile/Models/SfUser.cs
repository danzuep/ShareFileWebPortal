using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.ShareFile.Models
{
	/// <summary>
	/// Contact
	/// </summary>
	[Index(nameof(FirstName), nameof(LastName))]
	public record SfUser : SfPrincipal
	{
		public override int Discriminator { get; set; } = (int)SfPrincipalType.User;

		/// <summary>
		/// The first and last name of the user
		/// </summary>
		[StringLength(512)]
		public string FullName { get; set; }

		/// <summary>
		/// FirstName
		/// </summary>
		[StringLength(256)]
		public string FirstName { get; set; }

		/// <summary>
		/// LastName
		/// </summary>
		[StringLength(256)]
		public string LastName { get; set; }

		/// <summary>
		/// Company
		/// </summary>
		[StringLength(512)]
		public string Company { get; set; }

		public bool? IsConfirmed { get; set; }
		public bool? IsDisabled { get; set; }
		public int? TotalSharedFiles { get; set; }
		//public IEnumerable<string> Emails { get; set; }
		public DateTime? LastAnyLogin { get; set; }
	}
}