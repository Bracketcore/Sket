using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sket.Core.Misc
{
    public class NetworkDetector : IDisposable
    {
        public GeoPlugin info;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task Initialize()
        {
            using var axios = new System.Net.Http.HttpClient();
            var UserIp = await   axios.GetAsync("https://api.ipify.org/?format=json");
            
            var d = JsonConvert.DeserializeObject<Ipfy>( await UserIp.Content.ReadAsStringAsync());
           
            var network =  axios.GetAsync($"http://www.geoplugin.net/json.gp?ip={d.ip}").Result;
            info = JsonConvert.DeserializeObject<GeoPlugin>( network.Content.ReadAsStringAsync().Result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }

    internal class Ipfy
    {
        public string ip { get; set; }
    }

    public class GeoPlugin
    {
        public string geoplugin_request { get; set; }
        public int geoplugin_status { get; set; }
        public string geoplugin_delay { get; set; }
        public string geoplugin_credit { get; set; }
        public string geoplugin_city { get; set; }
        public string geoplugin_region { get; set; }
        public string geoplugin_regionCode { get; set; }
        public string geoplugin_regionName { get; set; }
        public string geoplugin_areaCode { get; set; }
        public string geoplugin_dmaCode { get; set; }
        public string geoplugin_countryCode { get; set; }
        public string geoplugin_countryName { get; set; }
        public int geoplugin_inEU { get; set; }
        public bool geoplugin_euVATrate { get; set; }
        public string geoplugin_continentCode { get; set; }
        public string geoplugin_continentName { get; set; }
        public string geoplugin_latitude { get; set; }
        public string geoplugin_longitude { get; set; }
        public string geoplugin_locationAccuracyRadius { get; set; }
        public string geoplugin_timezone { get; set; }
        public string geoplugin_currencyCode { get; set; }
        public string geoplugin_currencySymbol { get; set; }
        public string geoplugin_currencySymbol_UTF8 { get; set; }
        public double geoplugin_currencyConverter { get; set; }
    }
}