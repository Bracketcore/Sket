using System.Collections.Generic;
using MongoDB.Entities;

namespace Bracketcore.Sket.Entity
{
    /// <summary>
    ///     Abstract model for the User model
    /// </summary>
    [Name("Users")]
    public abstract class SketUserModel : SketPersistedModel
    {
        public SketUserModel()
        {
            DB.Index<SketUserModel>()
                .Key(o => o.Email, KeyType.Text)
                .Key(o => o.Username, KeyType.Text)
                .Option(o => o.Unique = true)
                .CreateAsync();
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Realm { get; set; }
        public bool EmailVerified { get; set; }
        public List<string> Role { get; set; }
        public string VerificationToken { get; set; }
        public double PhoneVerification { get; set; }
    }
}