using System.Collections.Generic;
using src;
using src.Interfaces;
using src.Models;

namespace tests.Mocks
{
    public class MockMessageService : IMessageService
    {
        public void SendErrorMessage(string subject, string errorMessage, NodeState state)
        {
            Subject = subject;
            ErrorMessage = errorMessage;
            State = state;
        }

        public NodeState State { get; set; }

        public string ErrorMessage { get; set; }

        public string Subject { get; set; }
    }
    
    public class MockLogger : ILogger
    {
        public List<string> Messages { get; set; }

        public MockLogger()
        {
            Messages = new List<string>();
        }
        
        public void Log(string msg)
        {
            Messages.Add(msg);
        }
    }
}