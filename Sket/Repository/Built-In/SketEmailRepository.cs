using Bracketcore.Sket.Entity;

namespace Bracketcore.Sket.Repository
{
    //Todo work on the send email services
    public class SketEmailRepository<T> where T : SketEmailModel
    {
        public bool TokenSend()
        {
            return true;
        }

        public bool WelcomeMessageSend()
        {
            return true;
        }

        public bool TransactionSend()
        {
            return true;
        }

        public bool LoginMessageSend()
        {
            return true;
        }
    }
}