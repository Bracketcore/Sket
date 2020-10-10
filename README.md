# **SKET**
Sket is an easy api creator framework which take care of a lot of aspnet core setup, speeding up your api creation in no time.
<hr/>

## Getting started with Sket

###### Installation
Install the nuget pagckage of sket 

>`Install-Package Sket`

Import namespace into you webapi project Startup.cs file

>`using Bracketcore.Sket;`

<hr/>


## Basic initialization

In your Startup.cs file in the ConfigurationServices method add the below code

    services.AddSket(new SketSettings
            {
                DatabaseName = "Database Name",
                JwtKey = "Your JWT Key",
                AuthType = AuthType.Jwt,
                DomainUrl = "Your domain Url",
                MongoSettings =MongoClientSettings.FromConnectionString(
                        Configuration["DatabaseSettings"]),
                ApiSetup = new List<ApiConfig>
                {
                    new ApiConfig
                    {
                        BaseUrl = "Your Api Url",
                        Endpoints = new List<string>
                        {
                            "Route01", "Route02"
                        }
                    }
                }
            });