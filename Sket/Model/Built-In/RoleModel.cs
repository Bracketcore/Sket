using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{
    [Name("Roles")]
    public class RoleModel: 
        SketPersistedModel
    {
        public  string Name { get; set; }
        public  string Description { get; set; }

        public RoleModel()
        {
            DB.Index<RoleModel>()
                .Key(n=>n.Name, KeyType.Text)
                .Option(o=> o.Unique = true)
                .Create();
        }
    }
}