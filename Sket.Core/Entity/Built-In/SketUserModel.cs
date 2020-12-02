using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Entities;

namespace Sket.Core.Entity
{
    /// <summary>
    ///     Abstract model for the User model
    /// </summary>
    [Name("Users")]
    public class SketUserModel : SketPersistedModel
    {
        public SketUserModel()
        {
            DB.Index<SketUserModel>()
                .Key(o => o.Email, KeyType.Text)
                .Key(o => o.Username, KeyType.Text)
                .Key(o => o.Phone, KeyType.Text)
                .Option(o => o.Unique = true)
                .CreateAsync();
        }

        public string Username { get; set; }

        [Required(ErrorMessage = "Password cant be empty")]
        public string Password { get; set; }

        [CompareProperty("Password", ErrorMessage = "Password dont match")]
        public string CPassword { get; set; }

        [Required] public string Email { get; set; }

        public string Phone { get; set; }
        public string Realm { get; set; }
        public bool EmailVerified { get; set; }
        public List<string> Role { get; set; }
        public string VerificationToken { get; set; }
        public double PhoneVerification { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}