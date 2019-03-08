using src.Interfaces;
using src.Models;

namespace tests.Mocks
{
    public class MockConfigProvider : IConfigurationProvider
    {
        public ExpectedNodeState ReadCurrentState()
        {
            return CurrentState;
        }

        public void WriteNewState(ExpectedNodeState newState)
        {
            CurrentState = newState;
        }

        public ExpectedNodeState CurrentState { get; set; }
    }
}