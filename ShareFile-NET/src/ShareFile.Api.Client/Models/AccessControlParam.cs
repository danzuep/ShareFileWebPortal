// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
//     
//	   Copyright (c) 2018 Citrix ShareFile. All rights reserved.
// </auto-generated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShareFile.Api.Client.Extensions;
using ShareFile.Api.Client.Exceptions;

namespace ShareFile.Api.Client.Models 
{
	/// <summary>
	/// Single AccessControl setting parameters for a bulk operation
	/// </summary>
	public class AccessControlParam : ODataObject 
	{
		/// <summary>
		/// AccessControl.Item is inherited from AccessControlsBulkParams and cannot be specified here
		/// </summary>
		public AccessControl AccessControl { get; set; }
		/// <summary>
		/// Defines whether this principal should receieve a notice on the permission grant.
		/// If not specified it is inherited AccessControlsBulkParams
		/// </summary>
		public bool? NotifyUser { get; set; }
		/// <summary>
		/// Custom notification message, if any
		/// If not specified it is inherited AccessControlsBulkParams
		/// </summary>
		public string NotifyMessage { get; set; }
		/// <summary>
		/// Defines whether this ACL change should be applied recursively
		/// </summary>
		public bool? Recursive { get; set; }

		public override void Copy(ODataObject source, JsonSerializer serializer)
		{
			if(source == null || serializer == null) return;
			base.Copy(source, serializer);

			var typedSource = source as AccessControlParam;
			if(typedSource != null)
			{
				AccessControl = typedSource.AccessControl;
				NotifyUser = typedSource.NotifyUser;
				NotifyMessage = typedSource.NotifyMessage;
				Recursive = typedSource.Recursive;
			}
			else
			{
				JToken token;
				if(source.TryGetProperty("AccessControl", out token) && token.Type != JTokenType.Null)
				{
					AccessControl = (AccessControl)serializer.Deserialize(token.CreateReader(), typeof(AccessControl));
				}
				if(source.TryGetProperty("NotifyUser", out token) && token.Type != JTokenType.Null)
				{
					NotifyUser = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("NotifyMessage", out token) && token.Type != JTokenType.Null)
				{
					NotifyMessage = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("Recursive", out token) && token.Type != JTokenType.Null)
				{
					Recursive = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
			}
		}
	}
}