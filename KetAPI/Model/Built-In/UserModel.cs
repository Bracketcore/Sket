using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{
    [Name("Users")]
    public  class UserModel: PersistedModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public One<RoleModel> Role { get; set; }
        public string VerificationToken { get; set; }
        public double PhoneVerification { get; set; }

        public UserModel()
        {
           DB.Index<UserModel>()
               .Key(o=>o.Email, KeyType.Text)
               .Key(o=>o.Username, KeyType.Text)
               .Option(o=> o.Unique = true)
               .Create();
           
        }
    }
}