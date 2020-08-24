using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{
    [Name("Users")]
    public abstract class SketUserModel : SketPersistedModel
    {
        public string Username { get; set; }
        public string NormalizedUsername { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public Many<RoleModel> Role { get; set; }
        public string VerificationToken { get; set; }
        public double PhoneVerification { get; set; }

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