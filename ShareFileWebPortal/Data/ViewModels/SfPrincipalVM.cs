using System;
using System.ComponentModel.DataAnnotations;

namespace ShareFileWebPortal.Data.ViewModels
{
	/// <summary>
	/// Represents an authenticated authority in ShareFile
	/// </summary>
	public record SfPrincipalVM
	{
		/// <summary>
		/// Unique record ID
		/// </summary>
		[MaxLength(36)]
		public string Uid { get; set; }

		/// <summary>
		/// Principal name
		/// </summary>
		[MaxLength(256)]
		public string Name { get; set; }

		/// <summary>
		/// Email address
		/// </summary>
		[MaxLength(512)]
		public string Email { get; set; }

		/// <summary>
		/// Username for the account - the value used for login. This is the same as Email for ShareFile accounts, but
		/// may be different on Connectors
		/// </summary>
		[MaxLength(512)]
		public string UserName { get; set; }

		/// <summary>
		/// Account domain
		/// </summary>
		[MaxLength(128)]
		public string Domain { get; set; }
	}
}