using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{
    [Name("Roles")]
    public sealed class RoleModel: PersistedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public RoleModel()
        {
            DB.Index<RoleModel>()
                .Key(n=>n.Name, KeyType.Text)
                .Option(o=> o.Unique = true)
                .Create();
        }
    }
}