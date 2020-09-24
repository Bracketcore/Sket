using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Bracketcore.Sket
{
    /// <summary>
    /// Core setup class for Sket to work.
    /// </summary>
    public class SketSettings : IDisposable
    {
        public AuthType AuthType { get; set; } = AuthType.Jwt;

        /// <summary>
        /// Set Database name
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Set JwtKey to setup your token creator
        /// </summary>
        public string JwtKey { get; set; }

        /// <summary>
        /// Default = false
        /// </summary>
        public bool EnableCamelCase { get; set; } = false;

        /// <summary>
        /// Set a list of URL to add to your Cors list. If empty Cors is disabled.
        /// </summary>
        public List<string> CorsDomains { get; set; }

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

    public enum AuthType
    {
        Cookie,
        Jwt
    }
}