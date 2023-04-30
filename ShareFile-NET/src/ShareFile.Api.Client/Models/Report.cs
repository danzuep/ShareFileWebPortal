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
	/// Represents a ShareFile Report: a set of configuration data used to generate JSON data
	/// </summary>
	public class Report : ODataObject 
	{
		/// <summary>
		/// The Account ID associated with this Report
		/// </summary>
		public string AccountId { get; set; }
		/// <summary>
		/// The name the Report will appear under in the Reporting UI
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// Access, Activity, Storage, Messaging, BandwidthDetail, BandwidthSummary, EncryptedEmail, StorageSummary, AccessChange, SharesSend, or SharesRequest
		/// </summary>
		public SafeEnum<ReportType> ReportType { get; set; }
		/// <summary>
		/// Account, Folder, or User
		/// </summary>
		public SafeEnum<ReportObjectType> ObjectType { get; set; }
		/// <summary>
		/// If ObjectType is specified, this is the Id of the object to run the Report against
		/// </summary>
		public string ObjectId { get; set; }
		/// <summary>
		/// (For non-Access reports) Specific, Today, Yesterday, ThisWeek, LastWeek, ThisMonth, LastMonth, or Last30Days
		/// </summary>
		public SafeEnum<ReportDateOption> DateOption { get; set; }
		/// <summary>
		/// (For Activity reports) A comma-delimited list of Login, LoginFail, LoginLocked, Download, View, WatermarkDownload, Upload, ZipUpload, Item_Delete, Item_Edit, Move, Item_Restore, NewFolder, NewNote, NewLink, DLP_Scan_Accept, DLP_Scan_Reject, DLP_Share_Allow, DLP_Share_Deny, Item_Archive, TFA_Login, TFA_LoginFail, TFA_InvalidCode, LoginFail_OAuthTokenExpired
		/// </summary>
		public string ActivityTypes { get; set; }
		/// <summary>
		/// If Specific is the DateOption, the beginning of the date range to report on
		/// </summary>
		public DateTime? StartDate { get; set; }
		/// <summary>
		/// If specific is the DateOption, the end of the data range to report on
		/// </summary>
		public DateTime? EndDate { get; set; }
		/// <summary>
		/// The last time this Report was run
		/// </summary>
		public DateTime? LastRunDate { get; set; }
		/// <summary>
		/// True if the Report should run regularly
		/// </summary>
		public bool? IsRecurring { get; set; }
		/// <summary>
		/// Should match IsRecurring
		/// </summary>
		public bool? IsScheduled { get; set; }
		/// <summary>
		/// If the ObjectType selected is Folder, whether or not subfolders should be included in the Report
		/// </summary>
		public bool? IncludeSubFolders { get; set; }
		/// <summary>
		/// True if the result of this Report should be directly saved to a ShareFile folder
		/// </summary>
		public bool? SaveToFolder { get; set; }
		/// <summary>
		/// If SaveToFolder is true, the format the Report should be saved in
		/// </summary>
		public string SaveFormat { get; set; }
		/// <summary>
		/// If SaveToFolder is true, the Id of the folder the Report should be saved in
		/// </summary>
		public string SaveFolderId { get; set; }
		/// <summary>
		/// The Item object representing the folder selected with SaveFolderId
		/// </summary>
		public Item SaveFolder { get; set; }
		/// <summary>
		/// The creator of the report
		/// </summary>
		public User Creator { get; set; }
		/// <summary>
		/// The user ID of the creator of the Report
		/// </summary>
		public string CreatorId { get; set; }
		/// <summary>
		/// An expandable property that includes all the ReportRecord objects associated with this Report
		/// </summary>
		public IEnumerable<ReportRecord> Records { get; set; }
		/// <summary>
		/// When this Report was originally created
		/// </summary>
		public DateTime? CreateDate { get; set; }
		/// <summary>
		/// If this Report is scheduled and recurring, when the Report should be run again - Once, Daily, Weekly, or Monthly
		/// </summary>
		public SafeEnum<ReportRunFrequency> Frequency { get; set; }
		/// <summary>
		/// If the Report is scheduled to run weekly, the day of the week to run on (Sunday is 0, Saturday is 6)
		/// </summary>
		public int? DayOfWeek { get; set; }
		/// <summary>
		/// If the Report is scheduled to run monthly, the day of the month to run on
		/// </summary>
		public int? DayOfMonth { get; set; }
		/// <summary>
		/// If true, the Report will send an email when it finishes executing
		/// </summary>
		public bool? EmailNotice { get; set; }
		/// <summary>
		/// If EmailNotice is true, the email address to notify
		/// </summary>
		public string EmailToNotify { get; set; }
		/// <summary>
		/// Specifies the custom Filters specific to each type of Report
		/// </summary>
		public ReportFilter Filter { get; set; }

		public override void Copy(ODataObject source, JsonSerializer serializer)
		{
			if(source == null || serializer == null) return;
			base.Copy(source, serializer);

			var typedSource = source as Report;
			if(typedSource != null)
			{
				AccountId = typedSource.AccountId;
				Title = typedSource.Title;
				ReportType = typedSource.ReportType;
				ObjectType = typedSource.ObjectType;
				ObjectId = typedSource.ObjectId;
				DateOption = typedSource.DateOption;
				ActivityTypes = typedSource.ActivityTypes;
				StartDate = typedSource.StartDate;
				EndDate = typedSource.EndDate;
				LastRunDate = typedSource.LastRunDate;
				IsRecurring = typedSource.IsRecurring;
				IsScheduled = typedSource.IsScheduled;
				IncludeSubFolders = typedSource.IncludeSubFolders;
				SaveToFolder = typedSource.SaveToFolder;
				SaveFormat = typedSource.SaveFormat;
				SaveFolderId = typedSource.SaveFolderId;
				SaveFolder = typedSource.SaveFolder;
				Creator = typedSource.Creator;
				CreatorId = typedSource.CreatorId;
				Records = typedSource.Records;
				CreateDate = typedSource.CreateDate;
				Frequency = typedSource.Frequency;
				DayOfWeek = typedSource.DayOfWeek;
				DayOfMonth = typedSource.DayOfMonth;
				EmailNotice = typedSource.EmailNotice;
				EmailToNotify = typedSource.EmailToNotify;
				Filter = typedSource.Filter;
			}
			else
			{
				JToken token;
				if(source.TryGetProperty("AccountId", out token) && token.Type != JTokenType.Null)
				{
					AccountId = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("Title", out token) && token.Type != JTokenType.Null)
				{
					Title = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("ReportType", out token) && token.Type != JTokenType.Null)
				{
					ReportType = (SafeEnum<ReportType>)serializer.Deserialize(token.CreateReader(), typeof(SafeEnum<ReportType>));
				}
				if(source.TryGetProperty("ObjectType", out token) && token.Type != JTokenType.Null)
				{
					ObjectType = (SafeEnum<ReportObjectType>)serializer.Deserialize(token.CreateReader(), typeof(SafeEnum<ReportObjectType>));
				}
				if(source.TryGetProperty("ObjectId", out token) && token.Type != JTokenType.Null)
				{
					ObjectId = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("DateOption", out token) && token.Type != JTokenType.Null)
				{
					DateOption = (SafeEnum<ReportDateOption>)serializer.Deserialize(token.CreateReader(), typeof(SafeEnum<ReportDateOption>));
				}
				if(source.TryGetProperty("ActivityTypes", out token) && token.Type != JTokenType.Null)
				{
					ActivityTypes = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("StartDate", out token) && token.Type != JTokenType.Null)
				{
					StartDate = (DateTime?)serializer.Deserialize(token.CreateReader(), typeof(DateTime?));
				}
				if(source.TryGetProperty("EndDate", out token) && token.Type != JTokenType.Null)
				{
					EndDate = (DateTime?)serializer.Deserialize(token.CreateReader(), typeof(DateTime?));
				}
				if(source.TryGetProperty("LastRunDate", out token) && token.Type != JTokenType.Null)
				{
					LastRunDate = (DateTime?)serializer.Deserialize(token.CreateReader(), typeof(DateTime?));
				}
				if(source.TryGetProperty("IsRecurring", out token) && token.Type != JTokenType.Null)
				{
					IsRecurring = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("IsScheduled", out token) && token.Type != JTokenType.Null)
				{
					IsScheduled = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("IncludeSubFolders", out token) && token.Type != JTokenType.Null)
				{
					IncludeSubFolders = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("SaveToFolder", out token) && token.Type != JTokenType.Null)
				{
					SaveToFolder = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("SaveFormat", out token) && token.Type != JTokenType.Null)
				{
					SaveFormat = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("SaveFolderId", out token) && token.Type != JTokenType.Null)
				{
					SaveFolderId = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("SaveFolder", out token) && token.Type != JTokenType.Null)
				{
					SaveFolder = (Item)serializer.Deserialize(token.CreateReader(), typeof(Item));
				}
				if(source.TryGetProperty("Creator", out token) && token.Type != JTokenType.Null)
				{
					Creator = (User)serializer.Deserialize(token.CreateReader(), typeof(User));
				}
				if(source.TryGetProperty("CreatorId", out token) && token.Type != JTokenType.Null)
				{
					CreatorId = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("Records", out token) && token.Type != JTokenType.Null)
				{
					Records = (IEnumerable<ReportRecord>)serializer.Deserialize(token.CreateReader(), typeof(IEnumerable<ReportRecord>));
				}
				if(source.TryGetProperty("CreateDate", out token) && token.Type != JTokenType.Null)
				{
					CreateDate = (DateTime?)serializer.Deserialize(token.CreateReader(), typeof(DateTime?));
				}
				if(source.TryGetProperty("Frequency", out token) && token.Type != JTokenType.Null)
				{
					Frequency = (SafeEnum<ReportRunFrequency>)serializer.Deserialize(token.CreateReader(), typeof(SafeEnum<ReportRunFrequency>));
				}
				if(source.TryGetProperty("DayOfWeek", out token) && token.Type != JTokenType.Null)
				{
					DayOfWeek = (int?)serializer.Deserialize(token.CreateReader(), typeof(int?));
				}
				if(source.TryGetProperty("DayOfMonth", out token) && token.Type != JTokenType.Null)
				{
					DayOfMonth = (int?)serializer.Deserialize(token.CreateReader(), typeof(int?));
				}
				if(source.TryGetProperty("EmailNotice", out token) && token.Type != JTokenType.Null)
				{
					EmailNotice = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("EmailToNotify", out token) && token.Type != JTokenType.Null)
				{
					EmailToNotify = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("Filter", out token) && token.Type != JTokenType.Null)
				{
					Filter = (ReportFilter)serializer.Deserialize(token.CreateReader(), typeof(ReportFilter));
				}
			}
		}
	}
}