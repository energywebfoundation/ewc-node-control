namespace src
{
    public class ContractWrapper
    {
        public ContractWrapper(string contractAddress)
        {
            
        }

        public ExpectedNodeState GetExpectedState()
        {
            return new ExpectedNodeState
            {
                DockerImage = "parity/parity:v2.3.3",
                DockerChecksum = "123456",
                IsSigning = true,
                ChainspecUrl = "https://",
                ChainspecChecksum = "123456",
                UpdateIntroducedBlock = 10
            };
        }
    }
}