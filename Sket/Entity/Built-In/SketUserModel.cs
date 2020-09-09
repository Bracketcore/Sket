using MongoDB.Entities;
using System.Collections.Generic;

namespace Bracketcore.Sket.Entity
{
    [Name("Users")]
    public abstract class SketUserModel : SketPersistedModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public List<string> Role { get; set; }
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