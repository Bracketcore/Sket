using MongoDB.Driver;

namespace Bracketcore.KetAPI
{
    public class SketSettings
    {
        public bool EnableCookies { get; set; } = true;
        public bool EnableJwt { get; set; } = false;
        public string DatabaseName { get; set; }

        public MongoClientSettings MongoSettings { get; set; } = new MongoClientSettings()
            { Server = new MongoServerAddress("localhost"), ReadConcern = ReadConcern.Majority };
    }
}