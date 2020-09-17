using AutoFixture.NUnit3;
using Bracketcore.Sket.Entity;
using NUnit.Framework;

namespace TestProject1.Entity
{
    public class RolesShould
    {
        
        [Theory, AutoData]
        public void PropertiesIsSet(string role, string name)
        {
            var roles = new SketRoleModel();

            roles.Description = role;
            roles.Name = name;

            Assert.AreEqual(role, roles.Description);
            Assert.AreEqual(name, roles.Name);
        }
    }
}