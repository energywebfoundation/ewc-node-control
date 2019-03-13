using src.Models;

namespace src.Interfaces
{
    public interface IConfigurationProvider
    {
        NodeState ReadCurrentState();
        void WriteNewState(NodeState newState);
    }
}