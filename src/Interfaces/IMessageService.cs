using src.Models;

namespace src
{
    public interface IMessageService
    {
        void SendMessage(string subject, string errorMEssage, ExpectedNodeState expectedState);
    }
}