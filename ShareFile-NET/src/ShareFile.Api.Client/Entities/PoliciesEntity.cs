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
using ShareFile.Api.Client;
using ShareFile.Api.Client.Extensions;
using ShareFile.Api.Client.Models;
using ShareFile.Api.Client.Requests;


namespace ShareFile.Api.Client.Entities
{
    public interface IPoliciesEntity : IEntityBase
    {
        
        /// <summary>
        /// Get List of Account Policies
        /// </summary>
        /// <remarks>
        /// The Policies endpoint returns the list of policies that have been defined for current account.
        /// </remarks>
        /// <returns>
        /// List of account policies
        /// </returns>
        IQuery<ODataFeed<Policy>> Get();
        
        /// <summary>
        /// Get Policy
        /// </summary>
        /// <remarks>
        /// Returns information for a specific Policy.
        /// </remarks>
        /// <param name="url"></param>
        /// <returns>
        /// A single Policy
        /// </returns>
        IQuery<Policy> Get(Uri url);
        
        /// <summary>
        /// Set user Policy
        /// </summary>
        /// <example>
        /// [{
        /// "Active": true,
        /// "Policy": {
        /// "Id": "123"
        /// }
        /// },
        /// {
        /// "Active": false,
        /// "Policy": {
        /// "Id": "456"
        /// }
        /// }]
        /// </example>
        /// <remarks>
        /// Replace all policies associated with user
        /// Leaving any category out will effectively disable PBA for that category. At most one policy per category is allowed.
        /// </remarks>
        /// <param name="url"></param>
        /// <param name="policyIds"></param>
        /// <returns>
        /// Returns user policies
        /// </returns>
        IQuery<ODataFeed<UserPolicy>> PatchByUser(Uri url, IEnumerable<UserPolicy> userPolicies);
        
        /// <summary>
        /// Create User Policy
        /// </summary>
        /// <example>
        /// {
        /// "Active": true,
        /// "Policy": {
        /// "Id": "123"
        /// }
        /// }
        /// </example>
        /// <remarks>
        /// Assigns a Policy to a user
        /// </remarks>
        /// <param name="url"></param>
        /// <param name="policy"></param>
        /// <returns>
        /// Returns newly assigned Policy
        /// </returns>
        IQuery<UserPolicy> CreateByUser(Uri url, UserPolicy policy);
        
        /// <summary>
        /// Delete User Policies
        /// </summary>
        /// <remarks>
        /// Remove user from policy based admin
        /// </remarks>
        /// <param name="url"></param>
        IQuery DeleteByUser(Uri url);
        IQuery DeleteByUser(Uri url, string id);
        
        /// <summary>
        /// Update a single UserPolicy
        /// </summary>
        /// <example>
        /// {
        /// "Active": false
        /// }
        /// </example>
        /// <remarks>
        /// Updates a single user Policy. Currently this only allows an update to the Active flag.
        /// </remarks>
        /// <param name="url"></param>
        IQuery<UserPolicy> UpdateByUser(Uri url, string id, UserPolicy updatedPolicy);
    }

    public class PoliciesEntity : EntityBase, IPoliciesEntity
    {
        public PoliciesEntity (IShareFileClient client)
            : base (client, "Policies")
        { }
        
        
        /// <summary>
        /// Get List of Account Policies
        /// </summary>
        /// <remarks>
        /// The Policies endpoint returns the list of policies that have been defined for current account.
        /// </remarks>
        /// <returns>
        /// List of account policies
        /// </returns>
        public IQuery<ODataFeed<Policy>> Get()
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query<ODataFeed<Policy>>(Client);
            sfApiQuery.From("Policies");
            sfApiQuery.HttpMethod = "GET";	
            return sfApiQuery;
        }
        
        /// <summary>
        /// Get Policy
        /// </summary>
        /// <remarks>
        /// Returns information for a specific Policy.
        /// </remarks>
        /// <param name="url"></param>
        /// <returns>
        /// A single Policy
        /// </returns>
        public IQuery<Policy> Get(Uri url)
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query<Policy>(Client);
            sfApiQuery.Uri(url);
            sfApiQuery.HttpMethod = "GET";	
            return sfApiQuery;
        }
        
        /// <summary>
        /// Set user Policy
        /// </summary>
        /// <example>
        /// [{
        /// "Active": true,
        /// "Policy": {
        /// "Id": "123"
        /// }
        /// },
        /// {
        /// "Active": false,
        /// "Policy": {
        /// "Id": "456"
        /// }
        /// }]
        /// </example>
        /// <remarks>
        /// Replace all policies associated with user
        /// Leaving any category out will effectively disable PBA for that category. At most one policy per category is allowed.
        /// </remarks>
        /// <param name="url"></param>
        /// <param name="policyIds"></param>
        /// <returns>
        /// Returns user policies
        /// </returns>
        public IQuery<ODataFeed<UserPolicy>> PatchByUser(Uri url, IEnumerable<UserPolicy> userPolicies)
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query<ODataFeed<UserPolicy>>(Client);
            sfApiQuery.Action("Policies");
            sfApiQuery.Uri(url);
            sfApiQuery.Body = userPolicies;
            sfApiQuery.HttpMethod = "PUT";	
            return sfApiQuery;
        }
        
        /// <summary>
        /// Create User Policy
        /// </summary>
        /// <example>
        /// {
        /// "Active": true,
        /// "Policy": {
        /// "Id": "123"
        /// }
        /// }
        /// </example>
        /// <remarks>
        /// Assigns a Policy to a user
        /// </remarks>
        /// <param name="url"></param>
        /// <param name="policy"></param>
        /// <returns>
        /// Returns newly assigned Policy
        /// </returns>
        public IQuery<UserPolicy> CreateByUser(Uri url, UserPolicy policy)
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query<UserPolicy>(Client);
            sfApiQuery.Action("Policies");
            sfApiQuery.Uri(url);
            sfApiQuery.Body = policy;
            sfApiQuery.HttpMethod = "POST";	
            return sfApiQuery;
        }
        
        /// <summary>
        /// Delete User Policies
        /// </summary>
        /// <remarks>
        /// Remove user from policy based admin
        /// </remarks>
        /// <param name="url"></param>
        public IQuery DeleteByUser(Uri url)
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query(Client);
            sfApiQuery.Action("Policies");
            sfApiQuery.Uri(url);
            sfApiQuery.HttpMethod = "DELETE";	
            return sfApiQuery;
        }
        public IQuery DeleteByUser(Uri url, string id)
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query(Client);
            sfApiQuery.Action("Policies");
            sfApiQuery.Uri(url);
            sfApiQuery.ActionIds(id);
            sfApiQuery.HttpMethod = "DELETE";	
            return sfApiQuery;
        }
        
        /// <summary>
        /// Update a single UserPolicy
        /// </summary>
        /// <example>
        /// {
        /// "Active": false
        /// }
        /// </example>
        /// <remarks>
        /// Updates a single user Policy. Currently this only allows an update to the Active flag.
        /// </remarks>
        /// <param name="url"></param>
        public IQuery<UserPolicy> UpdateByUser(Uri url, string id, UserPolicy updatedPolicy)
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query<UserPolicy>(Client);
            sfApiQuery.Action("Policies");
            sfApiQuery.Uri(url);
            sfApiQuery.ActionIds(id);
            sfApiQuery.Body = updatedPolicy;
            sfApiQuery.HttpMethod = "PATCH";	
            return sfApiQuery;
        }
    }
}
