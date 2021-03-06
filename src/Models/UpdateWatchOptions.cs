using src.Interfaces;

namespace src.Models
{
    /// <summary>
    /// Options and dependency injection for UpdateWatch
    /// </summary>
    public class UpdateWatchOptions
    {
        /// <summary>
        /// Address of the node control contract
        /// </summary>
        public string ContractAddress { get; set; } = string.Empty;

        /// <summary>
        /// The file to store the last checked block number in
        /// </summary>
        public string BlockNumberPersistFile { get; set; }

        /// <summary>
        /// url to the http endpoint for JSON-RPC
        /// </summary>
        public string RpcEndpoint { get; set; } = string.Empty;

        /// <summary>
        /// Ethereum address of the validator that is controlled by node control
        /// </summary>
        public string ValidatorAddress { get; set; } = string.Empty;

        /// <summary>
        /// Absolute path to the docker stack base path
        /// </summary>
        public string DockerStackPath { get; set; } = string.Empty;


        /// <summary>
        /// How log should the updater wait after applying the update and sending the update confirm transaction
        /// </summary>
        public int WaitTimeAfterUpdate { get; set; } = 30000;

        /// <summary>
        /// (DI) Instantiated ConfigurationProvider to use
        /// </summary>
        public IConfigurationProvider ConfigurationProvider { get; set; }

        /// <summary>
        /// (DI) Instantiated DockerComposeControl to use
        /// </summary>
        public IDockerControl DockerControl { get; set; }

        /// <summary>
        /// (DI) ContractWrapper to use
        /// </summary>
        public IContractWrapper ContractWrapper { get; set; }

        /// <summary>
        /// Path to the parity key file
        /// </summary>
        public string ValidatorKeyFile { get; set; }
    }
}