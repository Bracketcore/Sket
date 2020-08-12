namespace KetAPI.Model
{
    public abstract class UserModel: PersistedModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public bool isAdmin { get; set; }
        public string VerificationToken { get; set; }
        public double PhoneVerification { get; set; }
    }
}