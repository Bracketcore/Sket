namespace Bracketcore.KetAPI.Model
{
    public sealed class RoleModel: PersistedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}