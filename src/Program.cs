using System;
using System.Collections;
using System.IO;
using System.Threading;
using Docker.DotNet.Models;
using Nethereum.Contracts;
using src;
using src.Contract;
using src.Interfaces;
using src.Models;

namespace src
{
    internal static class Program
    {
        
        private static void Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();
            logger.Log("EWF NodeControl");
            
            // config stuff
            IDictionary env = Environment.GetEnvironmentVariables();
            UpdateWatchOptions watchOpts = ConfigBuilder.BuildConfigurationFromEnvironment(env);

            // Add dependencies
            watchOpts.ConfigurationProvider = new ConfigurationFileHandler(Path.Combine(watchOpts.DockerStackPath, ".env"));
            watchOpts.MessageService = new ConsoleMessageService();
            watchOpts.DockerControl = new LinuxDockerControl(logger);
            watchOpts.ContractWrapper = new ContractWrapper(watchOpts.ContractAddress,watchOpts.RpcEndpoint,watchOpts.ValidatorAddress);

            // instantiate the update watch
            UpdateWatch uw = new UpdateWatch(watchOpts,logger);

            // start watching
            uw.StartWatch();

            // Block main thread
            while (true)
            {
                Thread.Sleep(60000);
            }
            
        }
    }
}