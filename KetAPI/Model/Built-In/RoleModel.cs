namespace KetAPI.Model
{
    public abstract class RoleModel: PersistedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}