using System;
using System.Collections.Generic;
using Bracketcore.Sket;
using Bracketcore.Sket.Entity;
using NUnit.Framework;

namespace TestProject1
{
    public class SketConfigShould
    {
        [Test]
        public void ProteriesTest()
        {
            var config = new SketConfig();
            
            config.Context =new List<Type>();
            config.Roles = new List<SketRoleModel>();
            config.Settings = new SketSettings();
            
            Assert.NotNull(config.Context);
            Assert.NotNull(config.Roles);
            Assert.NotNull(config.Settings);
        }
    }
}