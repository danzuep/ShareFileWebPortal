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
	/// Device Log Entry
	/// </summary>
	public class DeviceLogEntry : ODataObject 
	{
		/// <summary>
		/// File name
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// File Id
		/// </summary>
		public string FileID { get; set; }
		/// <summary>
		/// Timestamp in milliseconds since epoch
		/// </summary>
		public long? Timestamp { get; set; }
		/// <summary>
		/// Account Id
		/// </summary>
		public string AccountID { get; set; }
		/// <summary>
		/// User Id
		/// </summary>
		public string UserID { get; set; }
		/// <summary>
		/// Action
		/// </summary>
		public SafeEnum<DeviceLogEntryAction> Action { get; set; }
		/// <summary>
		/// Additional Info
		/// </summary>
		public string AdditionalInfo { get; set; }

		public override void Copy(ODataObject source, JsonSerializer serializer)
		{
			if(source == null || serializer == null) return;
			base.Copy(source, serializer);

			var typedSource = source as DeviceLogEntry;
			if(typedSource != null)
			{
				FileName = typedSource.FileName;
				FileID = typedSource.FileID;
				Timestamp = typedSource.Timestamp;
				AccountID = typedSource.AccountID;
				UserID = typedSource.UserID;
				Action = typedSource.Action;
				AdditionalInfo = typedSource.AdditionalInfo;
			}
			else
			{
				JToken token;
				if(source.TryGetProperty("FileName", out token) && token.Type != JTokenType.Null)
				{
					FileName = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("FileID", out token) && token.Type != JTokenType.Null)
				{
					FileID = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("Timestamp", out token) && token.Type != JTokenType.Null)
				{
					Timestamp = (long?)serializer.Deserialize(token.CreateReader(), typeof(long?));
				}
				if(source.TryGetProperty("AccountID", out token) && token.Type != JTokenType.Null)
				{
					AccountID = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("UserID", out token) && token.Type != JTokenType.Null)
				{
					UserID = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("Action", out token) && token.Type != JTokenType.Null)
				{
					Action = (SafeEnum<DeviceLogEntryAction>)serializer.Deserialize(token.CreateReader(), typeof(SafeEnum<DeviceLogEntryAction>));
				}
				if(source.TryGetProperty("AdditionalInfo", out token) && token.Type != JTokenType.Null)
				{
					AdditionalInfo = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
			}
		}
	}
}