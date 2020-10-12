# **SKET**
Sket is an easy api creator framework which take care of a lot of aspnet projects setup and speeds up your api creation in no time.

Sket can be used on:
 - [x] Blazor server
 - [x] Blazor Webassembly 
 - [ ] Aspnet core WebApi Projects
 - [ ] Aspnet WebApi Projects 
 
 Sket can be with the following datasource:
 - [x] Mongodb [Powered By MongoDB.Entities](https://mongodb-entities.com/wiki/Get-Started.html)
 - [ ] Mysql
 - [ ] Postgres
 
 and more to come on users request.
<hr/>

## Getting started with Sket

###### Installation
Install the nuget pagckage of sket 

> `Install-Package Sket`

Import namespace into you webapi project Startup.cs file

```c#
 using Bracketcore.Sket;
```

<hr/>


## Basic initialization

In your Startup.cs file in the ConfigurationServices method add the below code

**_For Blazor setup_**
```c#
public void ConfigureServices(IServiceCollection services){
...
services.AddSket(new SketSettings
{
    DatabaseName = "Your Database Name",
    JwtKey = "Your JWT Key",
    AuthType = AuthType.Jwt,
    DomainUrl = "Your domain Url",
    MongoSettings = "Your mongodb connection string",
    ApiSetup = new List<ApiConfig>
    {
        new ApiConfig
        {
            BaseUrl = "Your Api Url",
            Endpoints = new List<string>
            {
                "Controller name 01", "Controller name 02"
            }
        }
    }
});
...
}
```
    