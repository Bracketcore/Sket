using MongoDB.Entities;

namespace Sket.Core.Models
{
    /// <summary>
    ///     Abstract model for the Role model
    /// </summary>
    [Name("Roles")]
    public class SketRoleModel :
        SketPersistedModel
    {
        public SketRoleModel()
        {
            DB.Index<SketRoleModel>()
                .Key(n => n.Name, KeyType.Text)
                .Option(o => o.Unique = true)
                .CreateAsync();
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}