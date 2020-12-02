using System.Collections.Generic;

namespace Sket.Core.HttpClient
{
    public interface IApiConfig
    {
        public string BaseUrl { get; set; }
        public List<string> Endpoints { get; set; }
    }
}