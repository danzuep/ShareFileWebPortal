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
	/// Represents AccessRight permissions on an IRM Classification
	/// </summary>
	public class IrmPrimaryAccessRightParams : ODataObject 
	{
		/// <summary>
		/// Allows user to view the documents
		/// </summary>
		public bool? View { get; set; }
		/// <summary>
		/// Allows user to use LightViewer client to view the documents
		/// </summary>
		public bool? LightViewer { get; set; }
		/// <summary>
		/// Allows user to print the documents
		/// </summary>
		public bool? Print { get; set; }
		/// <summary>
		/// Allows user to edit the documents
		/// </summary>
		public bool? Edit { get; set; }
		/// <summary>
		/// Allows user to have full control over the document.
		/// </summary>
		public bool? FullControl { get; set; }
		/// <summary>
		/// Allows user to copy the content into clipboard from the document.
		/// </summary>
		public bool? CopyData { get; set; }
		/// <summary>
		/// Allows user to use native screen capture tools.
		/// </summary>
		public bool? ScreenCapture { get; set; }
		/// <summary>
		/// Allows user to run macros. User cannot even view a macro file if this access right is set to false.
		/// </summary>
		public bool? Macro { get; set; }
		/// <summary>
		/// Allows user to access the documents offline
		/// </summary>
		public bool? OfflineAccess { get; set; }

		public override void Copy(ODataObject source, JsonSerializer serializer)
		{
			if(source == null || serializer == null) return;
			base.Copy(source, serializer);

			var typedSource = source as IrmPrimaryAccessRightParams;
			if(typedSource != null)
			{
				View = typedSource.View;
				LightViewer = typedSource.LightViewer;
				Print = typedSource.Print;
				Edit = typedSource.Edit;
				FullControl = typedSource.FullControl;
				CopyData = typedSource.CopyData;
				ScreenCapture = typedSource.ScreenCapture;
				Macro = typedSource.Macro;
				OfflineAccess = typedSource.OfflineAccess;
			}
			else
			{
				JToken token;
				if(source.TryGetProperty("View", out token) && token.Type != JTokenType.Null)
				{
					View = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("LightViewer", out token) && token.Type != JTokenType.Null)
				{
					LightViewer = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("Print", out token) && token.Type != JTokenType.Null)
				{
					Print = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("Edit", out token) && token.Type != JTokenType.Null)
				{
					Edit = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("FullControl", out token) && token.Type != JTokenType.Null)
				{
					FullControl = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("CopyData", out token) && token.Type != JTokenType.Null)
				{
					CopyData = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("ScreenCapture", out token) && token.Type != JTokenType.Null)
				{
					ScreenCapture = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("Macro", out token) && token.Type != JTokenType.Null)
				{
					Macro = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("OfflineAccess", out token) && token.Type != JTokenType.Null)
				{
					OfflineAccess = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
			}
		}
	}
}