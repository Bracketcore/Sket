using System;
using System.Collections.Generic;
using UnoRoute.Sket.Core.HttpClient;
using UnoRoute.Sket.Core.Manager;

namespace UnoRoute.Sket.Core.Init
{
    public interface ISketSetting : IDisposable
    {
        public AuthType AuthType { get; set; }
        public List<string> CorsDomains { get; set; }

        public string Datasource { get; set; }

        public string DBUsername { get; set; }
        public string DBPassword { get; set; }
        public int DBPort { get; set; }
        public bool EnableSwagger { get; set; }
        public string DomainUrl { get; set; }

        // public Type AppUserModel { get; set; }
        public string DatabaseName { get; set; }

        public string JwtKey { get; set; }
        public string ConnectionString { get; set; }
      
    }
}