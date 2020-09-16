using MongoDB.Entities;

namespace Bracketcore.Sket.Entity
{
    /// <summary>
    /// Abstract model for the Role model
    /// </summary>
    [Name("Roles")]
    public class SketRoleModel :
        SketPersistedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public SketRoleModel()
        {
            DB.Index<SketRoleModel>()
                .Key(n => n.Name, KeyType.Text)
                .Option(o => o.Unique = true)
                .CreateAsync();
        }
    }
}