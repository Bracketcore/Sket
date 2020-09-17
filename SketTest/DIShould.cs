using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bracketcore.Sket;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;
using NUnit.Framework;

namespace TestProject1
{
    public class DiShould
    {
        private IServiceCollection _services;


        [Test]
        public void DISetupOk()
        {
            _services = new Setup().init();

            Assert.IsTrue(_services.Count > 27);
        }

        [Test]
        public void SetRoleCount()
        {
            _services = new Setup().init(null,new List<string>(){"ehigiepaul"} );
            var getRoles = DB.Queryable<SketRoleModel>().FirstOrDefaultAsync().Result;

            Assert.IsNotNull(getRoles);
        }

 

        [Test]
        public void JwtKeyIsNull()
        {
            Assert.Throws<Exception>(() => { new Setup().init(""); });
        }

        [Test]
        public void TestInit()
        {
            var init = Sket.Init(new ServiceCollection(), new SketSettings()
            {
                DatabaseName = "test",
                JwtKey = "k/5USDsfCkqyOOJxrsCn8+zGRZ6OMReEwOI2I7QzDQM9B/c8QR6sK1x6hqlD3eePzT2BtQC6RC+DpQWctNpFow==",
            });

            Assert.IsNotNull(init);
        }
    }
}