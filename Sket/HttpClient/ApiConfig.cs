using System.Collections.Generic;

namespace Bracketcore.Sket
{
    public class ApiConfig:IApiConfig

    {
        public string BaseUrl { get; set; }
        public List<string> Endpoints { get; set; }
    }
}