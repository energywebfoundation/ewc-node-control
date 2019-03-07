using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Docker.DotNet;
using Docker.DotNet.Models;
using src.Contract;

namespace src
{
    internal static class Program
    {
        private static string _contractAddresss;
        private static string _stackPath;
        private static string _rpcEndpoint;
        private static string _validatorAddress;
        private static IConfigurationProvider _configHandler;

        private static void Main(string[] args)
        {
            Console.WriteLine("EWF NodeControl");

            // config stuff
            _contractAddresss = "0x";
            _stackPath = "./demo-stack";
            _rpcEndpoint = "http://localhost:8545";
            _validatorAddress = "0xc3681dfe99730eb45154208cba7b0df7e705f305";

            // Instantiate the contract
            _configHandler = new ConfigurationFileHandler(Path.Combine(_stackPath, ".env"));
            
            // test
          
           Timer checkTimer = new Timer(CheckForUpdates);
           checkTimer.Change(10000, 10000);

           Console.WriteLine("Listening for Updates....");

        }


        private static void CheckForUpdates(object state)
        {
            Console.WriteLine("Checking On-Chain for updates...");

            IContractWrapper cw = new ContractWrapper(_contractAddresss,_rpcEndpoint,_validatorAddress);
            StateCompare sc = new StateCompare(_configHandler);
            ExpectedNodeState expectedState = cw.GetExpectedState().Result; 

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
                try
                {
                    switch (act.Mode)
                    {
                        case UpdateMode.Docker:
                            UpdateDocker(act, expectedState);
                            break;
                        case UpdateMode.ChainSpec:
                            UpdateChainSpec(act);
                            break;
                        default:
                            throw new UpdateVerificationException("Unsupported update mode.");
                    }
                }
                catch (UpdateVerificationException uve)
                {
                    SendMail("Unable to verify update", uve.Message, expectedState);
                }
                catch (Exception ex)
                {
                    SendMail("Unknown error during update", ex.Message, expectedState);
                }
            }

            // wait until parity is back online
            Console.WriteLine("Waiting for updates to settle...");
            Thread.Sleep(20000);
            
            // Confirm update with tx through local parity
            cw.ConfirmUpdate().Wait();

        }

        private static void SendMail(string subject, string verifyErrorMessage, ExpectedNodeState expectedState)
        {
            // TODO: Send a message to the tech contact
        }

        private static void UpdateChainSpec(StateChangeAction act)
        {
            // verify https
            if (!act.Payload.StartsWith("https://"))
            {
                throw new UpdateVerificationException("Won't download chainspec from unencrypted URL");
            }

            string newChainSpec;

            // download chainspec
            using (HttpClient hc = new HttpClient())
            {
                newChainSpec = hc.GetStringAsync(act.Payload).Result;
            }


            // verify hash
            string newHash = HashString(newChainSpec);
            if (newHash != act.PayloadHash)
            {
                throw new UpdateVerificationException(
                    "Downloaded chainspec don't matches hash from chain");
            }

            // copy to final location
            string chainSpecPath = Path.Combine(_stackPath, "config/chainspec.json");

            if (!File.Exists(chainSpecPath))
            {
                throw new UpdateVerificationException("Unable to read current chainspec");
            }

            string fileTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

            // Backup current chainspec

            File.Move(chainSpecPath,
                Path.Combine(_stackPath, $"config/chainspec.json.{fileTimestamp}"));

            // write new
            File.WriteAllText(chainSpecPath, newChainSpec);

            // restart parity
            DockerControl.ApplyChangesToStack(_stackPath, true);
        }

        private static string HashString(string newChainSpec)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(newChainSpec));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                foreach (byte t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }  
                return builder.ToString();  
            }  
        }

        private static void UpdateDocker(StateChangeAction act, ExpectedNodeState expectedState)
        {
            // pull docker image
            DockerClient client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
                .CreateClient();


            Console.WriteLine($"Pulling new parity image [{act.Payload}] ..");

            // TODO: verify its a proper docker image tag

            Progress<JSONMessage> progress = new Progress<JSONMessage>();
            string msg = string.Empty;
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
                Tag = act.Payload.Split(':')[1]
            }, null, progress).Wait();

            // verify hash
            ImageInspectResponse inspectResult = client.Images.InspectImageAsync(act.Payload).Result;
            if (inspectResult.ID != act.PayloadHash)
            {
                Console.WriteLine("Image hashes don't match. Cancel update.");
                client.Images.DeleteImageAsync(act.Payload, new ImageDeleteParameters()).Wait();
                Console.WriteLine("Pulled imaged removed.");
                return;
            }
      
            // Image is legit. update docker compose
            Console.WriteLine("Image valid. Updating stack...");
            
            // modify docker-compose env file
            _configHandler.WriteNewState(expectedState);

            // restart/upgrade stack
            DockerControl.ApplyChangesToStack(_stackPath, false);
        }
    }
}
