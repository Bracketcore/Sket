using Sket.Core.Models;

namespace Sket.Core.Repository
{
    //Todo work on the send email services
    public class SketEmailRepository<T> where T : SketEmailModel
    {
        /// <summary>
        ///     send message
        /// </summary>
        /// <returns></returns>
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