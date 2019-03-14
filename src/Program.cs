using System;
using System.Collections;
using Docker.DotNet.Models;
using Nethereum.Contracts;
using src;
using src.Interfaces;
using src.Models;

namespace src
{
    internal static class Program
    {
        
        private static void Main(string[] args)
        {
            Console.WriteLine("EWF NodeControl");
            
            // config stuff
            IDictionary env = Environment.GetEnvironmentVariables();
            UpdateWatchOptions watchOpts = ConfigBuilder.BuildConfigurationFromEnvironment(env);

            // instantiate the update watch
            UpdateWatch uw = new UpdateWatch(watchOpts);
            
            // attach log output
            uw.OnLog += (sender, logArgs) => Console.WriteLine($"[WATCH] {logArgs.Message}");
            
            // start watching - Will block here
            uw.StartWatch();

        }
    }
}