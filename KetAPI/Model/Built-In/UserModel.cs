using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{
    [Name("Users")]
    public abstract class UserModel : PersistedModel
    {
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailVerified { get; set; }
        public virtual Many<RoleModel> Role { get; set; }
        public virtual string VerificationToken { get; set; }
        public virtual double PhoneVerification { get; set; }

        public UserModel()
        {
            DB.Index<UserModel>()
                .Key(o => o.Email, KeyType.Text)
                .Key(o => o.Username, KeyType.Text)
                .Option(o => o.Unique = true)
                .Create();
        }
    }
}