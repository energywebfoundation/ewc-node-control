using System;
using System.IO;
using Nethereum.Contracts;
using src;
using src.Interfaces;
using src.Models;

namespace src
{
    internal static class Program
    {
        private static string _contractAddresss;
        private static string _stackPath;
        private static string _rpcEndpoint;
        private static string _validatorAddress;

        private static void Main(string[] args)
        {
            Console.WriteLine("EWF NodeControl");

            // config stuff
            _contractAddresss = "0x";
            _stackPath = "./demo-stack";
            _rpcEndpoint = "http://localhost:8545";
            _validatorAddress = "0xc3681dfe99730eb45154208cba7b0df7e705f305";

            // fill the options
            UpdateWatchOptions watchOpts = new UpdateWatchOptions
            {
                RpcEndpoint = _rpcEndpoint,
                ContractAddress = _contractAddresss,
                ValidatorAddress = _validatorAddress,
                DockerStackPath = _stackPath,
                ConfigurationProvider = new ConfigurationFileHandler(Path.Combine(_stackPath, ".env")),
                MessageService = new ConsoleMessageService(),
                DockerComposeControl = new LinuxComposeControl()
            };
            
            // instantiate the update watch
            UpdateWatch uw = new UpdateWatch(watchOpts);
            
            // attach log output
            uw.OnLog += (sender, logArgs) => Console.WriteLine($"[WATCH] {logArgs.Message}");
            
            // start watching - Will block here
            uw.StartWatch();

            
        }
    }
}