namespace src
{
    public interface IConfigurationProvider
    {
        ExpectedNodeState ReadCurrentState();
        void WriteNewState(ExpectedNodeState newState);
    }
}