using src.Interfaces;

namespace src
{
    public class UpdateWatchOptions
    {
        public string ContractAddress { get; set; }
        public string RpcEndpoint { get; set; }
        public string ValidatorAddress { get; set; }

        public IConfigurationProvider ConfigurationProvider { get; set; }
        public IMessageService MessageService { get; set; }

        public IDockerComposeControl DockerControl { get; set; }
        public string DockerStackPath { get; set; }
    }
}