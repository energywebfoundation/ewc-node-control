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
            
            string contractAddresss = GetConfig(env,"CONTRACT_ADDRESS","0x0");
            string stackPath = GetConfig(env,"STACK_PATH","./demo-stack");
            string rpcEndpoint = GetConfig(env,"RPC_ENDPOINT","http://localhost:8545");
            string validatorAddress = GetConfig(env,"VALIDATOR_ADDRESS","0x0");
            
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