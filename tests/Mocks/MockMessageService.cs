using src.Interfaces;
using src.Models;

namespace tests.Mocks
{
    public class MockMessageService : IMessageService
    {
        public void SendMessage(string subject, string errorMEssage, ExpectedNodeState expectedState)
        {
            SendSubject = subject;
            SendErrorMessage = errorMEssage;
            SendExpectedState = expectedState;
        }

        public ExpectedNodeState SendExpectedState { get; set; }

        public string SendErrorMessage { get; set; }

        public string SendSubject { get; set; }
    }
}