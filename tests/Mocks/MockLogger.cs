using System.Collections.Generic;
using src;

namespace tests.Mocks
{
    public class MockLogger : ILogger
    {
        private List<string> Messages { get; }

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