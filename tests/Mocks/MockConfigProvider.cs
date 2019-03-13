using src.Interfaces;
using src.Models;

namespace tests.Mocks
{
    public class MockConfigProvider : IConfigurationProvider
    {
        public NodeState ReadCurrentState()
        {
            return CurrentState;
        }

        public void WriteNewState(NodeState newState)
        {
            CurrentState = newState;
        }

        public NodeState CurrentState { get; set; }
    }
}