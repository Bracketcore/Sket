using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{
    [Name("Roles")]
    public sealed class SketRoleModel: SketPersistedModel
    {
        public  string Name { get; set; }
        public  string Description { get; set; }

        public SketRoleModel()
        {
            DB.Index<SketRoleModel>()
                .Key(n=>n.Name, KeyType.Text)
                .Option(o=> o.Unique = true)
                .Create();
        }
    }
}