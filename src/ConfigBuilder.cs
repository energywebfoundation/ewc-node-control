using System;
using System.Collections;
using System.IO;
using src.Models;

namespace src
{
    public static class ConfigBuilder
    {
        private static string GetConfig(IDictionary env,string name, string defaultValue)
        {
            string value = (string) env[name];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
        
        public static UpdateWatchOptions BuildConfigurationFromEnvironment(IDictionary env)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env),"Environment dictionary can't be null");    
            }
            
            string contractAddresss = GetConfig(env,"CONTRACT_ADDRESS",string.Empty); // "0x0000000000000000000000000000000000000001"
            string stackPath = GetConfig(env,"STACK_PATH","./demo-stack");
            string rpcEndpoint = GetConfig(env,"RPC_ENDPOINT","http://localhost:8545");
            string validatorAddress = GetConfig(env,"VALIDATOR_ADDRESS",string.Empty); // "0x9935e9d4a208d13cd426d3bda7e6667faadb908d"
            
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