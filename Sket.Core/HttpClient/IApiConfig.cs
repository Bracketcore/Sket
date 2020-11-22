using System.Collections.Generic;

namespace UnoRoute.Sket.Core.HttpClient
{
    public interface IApiConfig
    {
        public string BaseUrl { get; set; }
        public List<string> Endpoints { get; set; }
    }
}