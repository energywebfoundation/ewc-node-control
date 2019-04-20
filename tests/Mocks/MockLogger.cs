using System.Collections.Generic;
using src;

namespace tests.Mocks
{
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

        public void Error(string subject, string errorMessage)
        {
            Messages.Add($"{subject}|${errorMessage}");
        }
    }
}