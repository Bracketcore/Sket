using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{
    [Name("Users")]
    public class SketUserModel : SketPersistedModel
    {
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailVerified { get; set; }
        public virtual One<SketRoleModel> Role { get; set; }
        public virtual string VerificationToken { get; set; }
        public virtual double PhoneVerification { get; set; }

        public SketUserModel()
        {
            DB.Index<SketUserModel>()
                .Key(o => o.Email, KeyType.Text)
                .Key(o => o.Username, KeyType.Text)
                .Option(o => o.Unique = true)
                .Create();
        }
    }
}