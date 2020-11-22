using System;
using System.Collections.Generic;

namespace UnoRoute.Sket.Core.HttpClient
{
    public class ApiConfig : IApiConfig, IDisposable

    {
        public string BaseUrl { get; set; }
        public List<string> Endpoints { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}