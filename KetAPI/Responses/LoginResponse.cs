using Bracketcore.KetAPI.Model;

namespace Bracketcore.KetAPI.Responses
{
    public class LoginResponse : PersistedModel
    {
        public string Message { get; set; }
        public string Tk { get; set; }
        public UserModel UserInfo { get; set; }
    }
}