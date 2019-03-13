using src;
using src.Interfaces;

namespace tests.Mocks
{
    public class MockDockerControl : IDockerComposeControl
    {
        public string SendPathToStack { get; set; }
        public bool SendRestartOnly { get; set; }
        public void ApplyChangesToStack(string pathToStack, bool restartOnly)
        {
            SendRestartOnly = restartOnly;
            SendPathToStack = pathToStack;
        }
    }
}