using src.Interfaces;
using src.Models;

namespace tests.Mocks
{
    public class MockMessageService : IMessageService
    {
        public void SendMessage(string subject, string errorMEssage, NodeState state)
        {
            SendSubject = subject;
            SendErrorMessage = errorMEssage;
            SendState = state;
        }

        public NodeState SendState { get; set; }

        public string SendErrorMessage { get; set; }

        public string SendSubject { get; set; }
    }
}