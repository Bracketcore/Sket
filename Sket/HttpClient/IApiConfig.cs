using System.Collections.Generic;

namespace Bracketcore.Sket.HttpClient
{
    public interface IApiConfig
    {
        public string BaseUrl { get; set; }
        public List<string> Endpoints { get; set; }
    }
}