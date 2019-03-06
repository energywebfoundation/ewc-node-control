using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json.Converters;

namespace src
{
    class Program
    {
        private static string _contractAddresss;
        private static string _pathToEnvFile;
        private static string _stackPath;

        static void Main(string[] args)
        {
            Console.WriteLine("EWF NodeControl");

            // config stuff
            _contractAddresss = "0x";
            _pathToEnvFile = "demo-env.txt";
            _stackPath = "./demo-stack";

            // Instantiate the contract
            
            
            // test
            UpdateDocker(new StateChangeAction
            {
                Mode = UpdateMode.Docker,
                Payload = "parity/parity:v2.3.4",
                PaylodSignature = "sha256:d7b09226f45f9d267006c43fe25ba8f20e518b8dcf5df0b93a6e7c310bb28e6c"
                
            }, new ExpectedNodeState
            {
                IsSigning = true,
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "sha256:d7b09226f45f9d267006c43fe25ba8f20e518b8dcf5df0b93a6e7c310bb28e6c"
            });
            return;
            
           Timer checkTimer = new Timer(CheckForUpdates);
           checkTimer.Change(10000, 10000);

           Console.WriteLine("Listening for Updates....");

        }


        private static void CheckForUpdates(object state)
        {
            Console.WriteLine("Checking On-Chain for updates...");

            ContractWrapper cw = new ContractWrapper(_contractAddresss);
            StateCompare sc = new StateCompare(new ConfigurationFileHandler(_pathToEnvFile));
            ExpectedNodeState expectedState = cw.GetExpectedState(); 

            // calculate action to 

            List<StateChangeAction> actions = sc.ComputeActionsFromState(expectedState);

            if (actions.Count == 0)
            {
                // No actions. Sleep.
                return;
            }
                
            // Process actions
            foreach(StateChangeAction act in actions)
            {
                if (act.Mode == UpdateMode.Docker)
                {
                    UpdateDocker(act, expectedState);
                } 
                else if (act.Mode == UpdateMode.ChainSpec)
                {
                    // download chainspec
                        
                    // verify siganture
                        
                    // copy to final location
                        
                    // restart parity
                        
                        
                }
            }
                
            // wait until parity is back online
                
            // Confirm update with tx through local parity
                
        }

        public static void UpdateDocker(StateChangeAction act, ExpectedNodeState expectedState)
        {
            // pull docker image
            DockerClient client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
                .CreateClient();


            Console.WriteLine($"Pulling new parity image [{act.Payload}] ..");

            // TODO: verify its a proper docker image tag

            var progress = new Progress<JSONMessage>();
            string msg = String.Empty;
            progress.ProgressChanged += (sender, message) =>
            {
                string newMsg = $"[DOCKER IMAGE PULL | {message.ID}] {message.Status}...";
                if (newMsg != msg)
                {
                    Console.WriteLine(newMsg);
                }

                msg = newMsg;
            };
            
            client.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = act.Payload.Split(':')[0],
                Tag = act.Payload.Split(':')[1],
            }, null, progress).Wait();

            // verify hash
            var inspectResult = client.Images.InspectImageAsync(act.Payload).Result;
            if (inspectResult.ID != act.PaylodSignature)
            {
                Console.WriteLine("Image signature don't match. Cancel update.");
                client.Images.DeleteImageAsync(act.Payload, new ImageDeleteParameters()).Wait();
                Console.WriteLine("Pulled imaged removed.");
                return;
            }
      
            // Image is legit. update docker compose
            Console.WriteLine("Image valid. Updating stack...");
            


            // modify docker-compse env file
            ConfigurationFileHandler cfh = new ConfigurationFileHandler(_pathToEnvFile);
            cfh.WriteNewState(expectedState);

            // restart/upgrade stack
            DockerControl.ApplyChangesToStack(_stackPath);
        }
    }
}
