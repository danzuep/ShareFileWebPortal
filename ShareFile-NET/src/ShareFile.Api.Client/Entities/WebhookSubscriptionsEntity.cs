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
    public interface IWebhookSubscriptionsEntity : IEntityBase
    {
        
        /// <summary>
        /// Gets a WebhookSubscription based on id
        /// </summary>
        /// <param name="url"></param>
        /// <returns>
        /// WebhookSubscription
        /// </returns>
        IQuery<WebhookSubscription> Get(Uri url);
        
        /// <summary>
        /// Gets all webhook subscriptions for the current user
        /// </summary>
        /// <returns>
        /// List of WebhookSubscription
        /// </returns>
        IQuery<ODataFeed<WebhookSubscription>> Get();
        
        /// <summary>
        /// Deletes a WebhookSubscription based on id
        /// </summary>
        /// <param name="url"></param>
        IQuery Delete(Uri url);
        
        /// <summary>
        /// Creates a WebhookSubscription
        /// </summary>
        /// <example>
        /// {
        /// "SubscriptionContext": {
        /// "ResourceType": "Folder",
        /// "ResourceId": "fo123"
        /// },
        /// "WebhookUrl": "https://webhook.com",
        /// "Events":
        /// [
        /// { "ResourceType":"File", "OperationName":"Upload" },
        /// { "ResourceType":"File", "OperationName":"Update" },
        /// { "ResourceType":"File", "OperationName":"Download" },
        /// { "ResourceType":"File", "OperationName":"Delete" },
        /// ]
        /// }
        /// </example>
        /// <remarks>
        /// The above example subscribes `https://webhook.com` endpoint to be called when a File Upload, Update, Download or Delete
        /// event happened for folder `fo123`.
        /// Note: Context Resource Id is required for all but the account context.
        /// The MasterAdmin role is required to create account context subscriptions.
        /// </remarks>
        /// <returns>
        /// WebhookSubscription
        /// </returns>
        IQuery<WebhookSubscription> Create(WebhookSubscription subscription);
    }

    public class WebhookSubscriptionsEntity : EntityBase, IWebhookSubscriptionsEntity
    {
        public WebhookSubscriptionsEntity (IShareFileClient client)
            : base (client, "WebhookSubscriptions")
        { }
        
        
        /// <summary>
        /// Gets a WebhookSubscription based on id
        /// </summary>
        /// <param name="url"></param>
        /// <returns>
        /// WebhookSubscription
        /// </returns>
        public IQuery<WebhookSubscription> Get(Uri url)
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query<WebhookSubscription>(Client);
            sfApiQuery.Uri(url);
            sfApiQuery.HttpMethod = "GET";	
            return sfApiQuery;
        }
        
        /// <summary>
        /// Gets all webhook subscriptions for the current user
        /// </summary>
        /// <returns>
        /// List of WebhookSubscription
        /// </returns>
        public IQuery<ODataFeed<WebhookSubscription>> Get()
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query<ODataFeed<WebhookSubscription>>(Client);
            sfApiQuery.From("WebhookSubscriptions");
            sfApiQuery.HttpMethod = "GET";	
            return sfApiQuery;
        }
        
        /// <summary>
        /// Deletes a WebhookSubscription based on id
        /// </summary>
        /// <param name="url"></param>
        public IQuery Delete(Uri url)
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query(Client);
            sfApiQuery.Uri(url);
            sfApiQuery.HttpMethod = "DELETE";	
            return sfApiQuery;
        }
        
        /// <summary>
        /// Creates a WebhookSubscription
        /// </summary>
        /// <example>
        /// {
        /// "SubscriptionContext": {
        /// "ResourceType": "Folder",
        /// "ResourceId": "fo123"
        /// },
        /// "WebhookUrl": "https://webhook.com",
        /// "Events":
        /// [
        /// { "ResourceType":"File", "OperationName":"Upload" },
        /// { "ResourceType":"File", "OperationName":"Update" },
        /// { "ResourceType":"File", "OperationName":"Download" },
        /// { "ResourceType":"File", "OperationName":"Delete" },
        /// ]
        /// }
        /// </example>
        /// <remarks>
        /// The above example subscribes `https://webhook.com` endpoint to be called when a File Upload, Update, Download or Delete
        /// event happened for folder `fo123`.
        /// Note: Context Resource Id is required for all but the account context.
        /// The MasterAdmin role is required to create account context subscriptions.
        /// </remarks>
        /// <returns>
        /// WebhookSubscription
        /// </returns>
        public IQuery<WebhookSubscription> Create(WebhookSubscription subscription)
        {
            var sfApiQuery = new ShareFile.Api.Client.Requests.Query<WebhookSubscription>(Client);
            sfApiQuery.From("WebhookSubscriptions");
            sfApiQuery.Body = subscription;
            sfApiQuery.HttpMethod = "POST";	
            return sfApiQuery;
        }
    }
}
