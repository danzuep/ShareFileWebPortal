using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.ShareFile.Models
{
	/// <summary>
	/// Represents an authenticated authority in ShareFile
	/// </summary>
	public record SfPrincipal : EntityUidBase
	{
		/// <summary>
		/// Discriminator https://docs.microsoft.com/en-nz/ef/core/modeling/inheritance
		/// </summary>
		[DefaultValue(0)]
		public virtual int Discriminator { get; set; } //= (int)SfPrincipalType.Principal;

		/// <summary>
		/// User name
		/// </summary>
		[StringLength(256)]
		public string Name { get; set; }

		/// <summary>
		/// Email address
		/// </summary>
		[StringLength(512)]
		public string Email { get; set; }

		/// <summary>
		/// Username for the account - the value used for login. This is the same as Email for ShareFile accounts, but
		/// may be different on Connectors
		/// </summary>
		[StringLength(512)]
		[Column("Username")]
		public string Username { get; set; }

		/// <summary>
		/// Account domain
		/// </summary>
		[StringLength(128)]
		public string Domain { get; set; }

		//public virtual ICollection<SfGroup> Groups { get; set; }
	}

	public enum SfPrincipalType
	{
		Principal = 0,
		User = 1,
		Group = 2,
		Contact = 3
	}
}