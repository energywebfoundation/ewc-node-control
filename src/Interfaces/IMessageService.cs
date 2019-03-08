using src.Models;

namespace src.Interfaces
{
    public interface IMessageService
    {
        void SendMessage(string subject, string errorMEssage, ExpectedNodeState expectedState);
    }
}