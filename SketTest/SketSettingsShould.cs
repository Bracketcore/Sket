using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.NUnit3;
using Bracketcore.Sket;
using Bracketcore.Sket.Entity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NUnit.Framework;

namespace TestProject1
{
    public class SketSettingsShould
    {

        [Theory, AutoData]
        public void PropertiesNotNull(string str, bool boo)
        {
            var sut = new SketSettings();

            sut.CorsDomains = new List<string>();
            sut.CorsDomains = new List<string>();
            sut.DatabaseName = str;
            sut.EnableCookies = boo;
            sut.EnableJwt = boo;
            sut.EnableCamelCase = boo;
            sut.MongoSettings = new MongoClientSettings();
            
            Assert.AreEqual(str, sut.DatabaseName);
            Assert.AreEqual(boo, sut.EnableJwt);
            Assert.AreEqual(boo, sut.EnableCookies);
            Assert.AreEqual(boo, sut.EnableCamelCase);
            Assert.IsNotNull(sut.MongoSettings);
        }
    }
}