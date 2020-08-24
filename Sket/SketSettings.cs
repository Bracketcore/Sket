using MongoDB.Driver;

namespace Bracketcore.KetAPI
{
    public class SketSettings
    {
        public bool EnableCookies { get; set; } = true;
        public bool EnableJwt { get; set; } = false;
        public string DatabaseName { get; set; }
        /// <summary>
        /// Use the JwtKey to setup your token creator
        /// </summary>
        public string JwtKey { get; set; }

        public bool EnableCamelCase { get; set; } = false;

        public MongoClientSettings MongoSettings { get; set; } = new MongoClientSettings()
        { Server = new MongoServerAddress("localhost"), ReadConcern = ReadConcern.Majority };
    }
}