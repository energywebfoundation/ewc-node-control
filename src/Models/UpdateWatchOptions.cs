using src.Interfaces;

namespace src.Models
{
    public class UpdateWatchOptions
    {
        public string ContractAddress { get; set; } = string.Empty;
        public string RpcEndpoint { get; set; } = string.Empty;
        public string ValidatorAddress { get; set; } = string.Empty;
        public string DockerStackPath { get; set; } = string.Empty;
        
        public IConfigurationProvider ConfigurationProvider { get; set; }
        public IMessageService MessageService { get; set; }
        public IDockerComposeControl DockerControl { get; set; }
        
    }
}