using src.Interfaces;

namespace src.Models
{
    /// <summary>
    /// Options and dependency injection for UpdateWatch
    /// </summary>
    public class UpdateWatchOptions
    {
        /// <summary>
        /// Address of the nodecontrol contract
        /// </summary>
        public string ContractAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// url to the http endpoint for JSON-RPC
        /// </summary>
        public string RpcEndpoint { get; set; } = string.Empty;
        
        /// <summary>
        /// Ethereum address of the validator that is controlled by node control 
        /// </summary>
        public string ValidatorAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// Absolut path to the docker stack base path
        /// </summary>
        public string DockerStackPath { get; set; } = string.Empty;
        
        /// <summary>
        /// (DI) Instantiated ConfigurationProvider to use
        /// </summary>
        public IConfigurationProvider ConfigurationProvider { get; set; }
        
        /// <summary>
        /// (DI) Instantiated MessageService to use
        /// </summary>
        public IMessageService MessageService { get; set; }
        /// <summary>
        /// (DI) Instantiated DockerComposeControl to use
        /// </summary>
        public IDockerControl DockerControl { get; set; }
        
        /// <summary>
        /// (DI) ContractWrapper to use
        /// </summary>
        public IContractWrapper ContractWrapper { get; set; }
        
       
        
    }
}