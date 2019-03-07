using src.Models;

namespace src.Interfaces
{
    public interface IConfigurationProvider
    {
        ExpectedNodeState ReadCurrentState();
        void WriteNewState(ExpectedNodeState newState);
    }
}