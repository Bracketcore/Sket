using System.Collections.Generic;
using MongoDB.Driver;

namespace Bracketcore.Sket
{
    /// <summary>
    /// Core setup class for Sket to work.
    /// </summary>
    public class SketSettings
    {
        /// <summary>
        /// Default = true
        /// </summary>
        public bool EnableCookies { get; set; } = true;
        /// <summary>
        /// Set Jwt features.
        /// Default = false
        /// </summary>
        public bool EnableJwt { get; set; } = false;
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
        { Server = new MongoServerAddress("localhost"), ReadConcern = ReadConcern.Majority };
    }
}