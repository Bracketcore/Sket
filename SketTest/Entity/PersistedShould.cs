using System;
using AutoFixture.NUnit3;
using Bracketcore.Sket.Entity;
using MongoDB.Bson;
using MongoDB.Entities;
using NUnit.Framework;

namespace TestProject1.Entity
{
    public class PersistedShould
    {
        [Theory, AutoData]
        public void PropertiesIsSet(DateTime role, string name)
        {
            var roles = new TestPersisted();

            roles.ModifiedOn = role;
            roles.OwnerID = name;

            Assert.AreEqual(role, roles.ModifiedOn);
            Assert.AreEqual(name, roles.OwnerID.ID);
        }
    }

   public class TestPersisted : SketPersistedModel
    {
        
    }
}