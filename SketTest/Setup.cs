using System;
using System.Collections.Generic;
using System.IO;
using Bracketcore.Sket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TestProject1
{
    public class Setup
    {

        public IServiceCollection init(string key = null , List<string> cors = null)
        {
            var services = new ServiceCollection()
                    .AddSket(new SketSettings()
                    {
                        DatabaseName = "test",
                        CorsDomains = cors,
                        JwtKey = key is null ?
                            "k/5USDsfCkqyOOJxrsCn8+zGRZ6OMReEwOI2I7QzDQM9B/c8QR6sK1x6hqlD3eePzT2BtQC6RC+DpQWctNpFow==" : null,
                    })
                    .AddOptions()
                ;

            services.BuildServiceProvider();

            return services;
        }
    }
}