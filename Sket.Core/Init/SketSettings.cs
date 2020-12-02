using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sket.Core.Manager;

namespace Sket.Core.Init
{
    /// <summary>
    ///     Core setup class for Sket to work.
    /// </summary>
    public class SketSettings : ISketSetting
    {
        public SketAppInfo AppInfo { get; set; }
        public AuthType AuthType { get; set; } = AuthType.Jwt;

        /// <summary>
        ///     Set Database name
        /// </summary>
        [Required]
        public string DatabaseName { get; set; }

        /// <summary>
        ///     Set JwtKey to setup your token creator
        /// </summary>
        [Required(ErrorMessage = "Empty Jtwkey, Hit Generate")]
        public string JwtKey { get; set; }

        /// <summary>
        ///     Set the application domain which will be used for the JWT issuer and audience
        /// </summary>
        public string DomainUrl { get; set; }


        /// <summary>
        ///     Set a list of URL to add to your Cors list. If empty Cors is disabled.
        /// </summary>
        public List<string> CorsDomains { get; set; }

        [Required(ErrorMessage = "Choose a datasource")]
        public string Datasource { get; set; }


        public string DBUsername { get; set; }

        public string DBPassword { get; set; }

        [Required(ErrorMessage = "Port Number needed")]
        public int DBPort { get; set; }

        public bool EnableSwagger { get; set; }

        /// <summary>
        ///     Set your MongoClientSettings
        /// </summary>
        [Required(ErrorMessage = "Enter host or connection string")]
        public string ConnectionString { get; set; }

        public void Dispose()
        {
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}