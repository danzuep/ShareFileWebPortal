using System;
using System.ComponentModel.DataAnnotations;

namespace ShareFileWebPortal.Data.ViewModels
{
	public record SfContactVM : SfPrincipalVM
	{
		/// <summary>
		/// The first and last name of the user
		/// </summary>
		[MaxLength(512)]
		public string FullName => $"{LastName}, {FirstName}";

		/// <summary>
		/// FirstName
		/// </summary>
		[MaxLength(256)]
		public string FirstName { get; set; }

		/// <summary>
		/// LastName
		/// </summary>
		[MaxLength(256)]
		public string LastName { get; set; }

		/// <summary>
		/// Company
		/// </summary>
		[MaxLength(512)]
		public string Company { get; set; }

		public int? MemberCount { get; set; }
	}
}