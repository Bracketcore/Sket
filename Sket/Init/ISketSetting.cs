using System;
using System.Collections.Generic;
using Bracketcore.Sket.HttpClient;
using Bracketcore.Sket.Manager;

namespace Bracketcore.Sket.Init
{
    public interface ISketSetting : IDisposable
    {
        public AuthType AuthType { get; set; }
        public List<string> CorsDomains { get; set; }

        public string DomainUrl { get; set; }

        // public Type AppUserModel { get; set; }
        public string DatabaseName { get; set; }
        public bool EnableCamelCase { get; set; }
        public string JwtKey { get; set; }
        public string MongoConnectionString { get; set; }
        public List<ApiConfig> ApiSetup { get; set; }
    }
}