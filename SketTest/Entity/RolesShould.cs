using AutoFixture.NUnit3;
using Bracketcore.Sket.Entity;
using NUnit.Framework;

namespace TestProject1.Entity
{
    public class RolesShould
    {
        
        [Theory, AutoData]
        public void PropertiesIsSet(string str)
        {
            var roles = new SketRoleModel();

            roles.Description = str;
            roles.Name = str;
            roles.Name = str;
            
        }
    }
}