﻿ using System;
using System.Collections.Generic;
using Bracketcore.Sket.Entity;
using MongoDB.Driver;

namespace Bracketcore.Sket
{
    /// <summary>
    /// Core setup class for Sket to work.
    /// </summary>
    public class SketSettings :ISketSetting
    {
        public AuthType AuthType { get; set; } = AuthType.Cookie;

        /// <summary>
        /// Set Database name
        /// </summary>
        public string DatabaseName { get; set; }
        
        /// <summary>
        /// Set JwtKey to setup your token creator
        /// </summary>
        public string JwtKey { get; set; }

        /// <summary>
        /// Set the application domain which will be used for the JWT issuer and audience
        /// </summary>
        public string DomainUrl { get; set; }

        /// <summary>
        /// Default = false
        /// </summary>
        public bool EnableCamelCase { get; set; } = false;

        /// <summary>
        /// Set a list of URL to add to your Cors list. If empty Cors is disabled.
        /// </summary>
        public List<string> CorsDomains { get; set; }

        /// <summary>
        /// Set a list of external api routes
        /// </summary>
        public List<ApiConfig> ApiSetup { get; set; }

        /// <summary>
        /// Set your MongoClientSettings
        /// </summary>
        public MongoClientSettings MongoSettings { get; set; } = new MongoClientSettings()
            {Server = new MongoServerAddress("localhost"), ReadConcern = ReadConcern.Majority};

     
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}