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
	public record SfContact : SfPrincipal
	{
		//public virtual SfUser User { get; set; }
		public override int Discriminator { get; set; } = (int)SfPrincipalType.Contact;

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

		public int? MemberCount { get; set; }
	}
}