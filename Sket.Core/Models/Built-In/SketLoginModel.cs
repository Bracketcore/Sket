using System.ComponentModel.DataAnnotations;

namespace Sket.Core.Models
{
    public class SketLoginModel
    {
        public string Username { get; set; }

        [Required] public string Password { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
    }
}