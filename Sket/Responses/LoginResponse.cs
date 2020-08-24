using Bracketcore.KetAPI.Model;

namespace Bracketcore.KetAPI.Responses
{
    public class LoginResponse : SketPersistedModel
    {
        public string Message { get; set; }
        public string Tk { get; set; }
        public SketUserModel UserInfo { get; set; }
    }
}