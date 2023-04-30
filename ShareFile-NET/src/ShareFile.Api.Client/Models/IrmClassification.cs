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
	/// Represents Irm Classification. ShareFile Admin can define properties on this Irm Classification.
	/// </summary>
	public class IrmClassification : ODataObject 
	{
		/// <summary>
		/// Name of the Irm Classification
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Description of the Irm Classification if any
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// AccessRight permissions on an IRM Classification
		/// </summary>
		public IrmPrimaryAccessRightParams PrimaryAccessRightParams { get; set; }
		/// <summary>
		/// Use this flag to enable/ disable the Irm Classification.
		/// If disabled, this Irm Classification won't show up in the List of active IrmClassifications on an account.
		/// </summary>
		public bool? IsEnabled { get; set; }
		/// <summary>
		/// Defines the numner of days after which the user won't be access the documents protected with this classification.
		/// </summary>
		public int? NumberOfProtectionDays { get; set; }

		public override void Copy(ODataObject source, JsonSerializer serializer)
		{
			if(source == null || serializer == null) return;
			base.Copy(source, serializer);

			var typedSource = source as IrmClassification;
			if(typedSource != null)
			{
				Name = typedSource.Name;
				Description = typedSource.Description;
				PrimaryAccessRightParams = typedSource.PrimaryAccessRightParams;
				IsEnabled = typedSource.IsEnabled;
				NumberOfProtectionDays = typedSource.NumberOfProtectionDays;
			}
			else
			{
				JToken token;
				if(source.TryGetProperty("Name", out token) && token.Type != JTokenType.Null)
				{
					Name = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("Description", out token) && token.Type != JTokenType.Null)
				{
					Description = (string)serializer.Deserialize(token.CreateReader(), typeof(string));
				}
				if(source.TryGetProperty("PrimaryAccessRightParams", out token) && token.Type != JTokenType.Null)
				{
					PrimaryAccessRightParams = (IrmPrimaryAccessRightParams)serializer.Deserialize(token.CreateReader(), typeof(IrmPrimaryAccessRightParams));
				}
				if(source.TryGetProperty("IsEnabled", out token) && token.Type != JTokenType.Null)
				{
					IsEnabled = (bool?)serializer.Deserialize(token.CreateReader(), typeof(bool?));
				}
				if(source.TryGetProperty("NumberOfProtectionDays", out token) && token.Type != JTokenType.Null)
				{
					NumberOfProtectionDays = (int?)serializer.Deserialize(token.CreateReader(), typeof(int?));
				}
			}
		}
	}
}