using System;
using System.Collections;
using System.IO;
using src.Models;

namespace src
{
    /// <summary>
    /// Builds the options for the update watch
    /// </summary>
    public static class ConfigBuilder
    {
        /// <summary>
        /// Read the config from the environment
        /// </summary>
        /// <param name="env">Environment variables dictionary</param>
        /// <param name="name">Name of the variable to read</param>
        /// <param name="defaultValue">Value to return when variable was not found in environment dictionary</param>
        /// <returns></returns>
        private static string GetConfig(IDictionary env,string name, string defaultValue)
        {
            string value = (string) env[name];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
        
        /// <summary>
        /// Build the UpdateWatchOptions from the environment
        /// </summary>
        /// <param name="env">Environment Dictionary to read the configuration from</param>
        /// <returns>Populated update watch options</returns>
        /// <exception cref="ArgumentNullException">Thrown when dictionary is null</exception>
        public static UpdateWatchOptions BuildConfigurationFromEnvironment(IDictionary env)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env),"Environment dictionary can't be null");    
            }
            
            string contractAddresss = GetConfig(env,"CONTRACT_ADDRESS","0x1204700000000000000000000000000000000007"); // "0x0000000000000000000000000000000000000001"
            string stackPath = GetConfig(env,"STACK_PATH","./demo-stack");
            string rpcEndpoint = GetConfig(env,"RPC_ENDPOINT","http://35.177.248.210:8545");
            string validatorAddress = GetConfig(env,"VALIDATOR_ADDRESS","0xebee2fc556975c3dd50c17d13a15af535fb7bbb3"); // "0x9935e9d4a208d13cd426d3bda7e6667faadb908d"
            
            return new UpdateWatchOptions
            {
                RpcEndpoint = rpcEndpoint,
                ContractAddress = contractAddresss,
                ValidatorAddress = validatorAddress,
                DockerStackPath = stackPath
            };
        }
    }
}