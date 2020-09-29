using System.Collections.Generic;

namespace Bracketcore.Sket
{
    public interface IApiConfig
    {
        public string BaseUrl { get; set; }
        public List<string> Endpoints { get; set; }
    }
}