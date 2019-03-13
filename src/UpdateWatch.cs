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
using src.Interfaces;
using src.Models;

namespace src
{
    public class UpdateWatch
    {
        internal event EventHandler<LogArgs> OnLog
        {
            add => _onLog += value;
            // TODO: Rider complains about delegate subtraction - find better way - https://www.jetbrains.com/help/rider/DelegateSubtraction.html
            remove => _onLog -= value;
        }

        private EventHandler<LogArgs> _onLog;
        private readonly IContractWrapper _cw;
        private readonly StateCompare _sc;
        private readonly IMessageService _msgService;
        private readonly string _stackPath;
        private readonly IConfigurationProvider _configProvider;
        private readonly IDockerComposeControl _dcc;

        public UpdateWatch(UpdateWatchOptions opts)
        {
            _cw = new ContractWrapper(opts.ContractAddress, opts.RpcEndpoint, opts.ValidatorAddress);
            _sc = new StateCompare(opts.ConfigurationProvider);
            _msgService = opts.MessageService;
            _stackPath = opts.DockerStackPath;
            _configProvider = opts.ConfigurationProvider;
            _dcc = opts.DockerControl;
        }


        public void StartWatch()
        {
            Log("Starting watch");
            Timer checkTimer = new Timer(CheckForUpdates);
            checkTimer.Change(10000, 10000);
        }

        private void Log(string message)
        {
            _onLog(this, new LogArgs(message));
        }

        private void CheckForUpdates(object state)
        {
            Log("Checking On-Chain for updates.");
            
            if(!_cw.HasNewUpdate().Result)
            {
                // No new update events on chain
                return;
            }
                
            ExpectedNodeState expectedState = _cw.GetExpectedState().Result;

            // calculate action to 

            List<StateChangeAction> actions = _sc.ComputeActionsFromState(expectedState);

            if (actions.Count == 0)
            {
                // No actions. Sleep.
                return;
            }

            // Process actions
            foreach (StateChangeAction act in actions)
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
                    _msgService.SendMessage("Unable to verify update", uve.Message, expectedState);
                }
                catch (Exception ex)
                {
                    _msgService.SendMessage("Unknown error during update", ex.Message, expectedState);
                }
            }

            // wait until parity is back online
            Log("Waiting 20 seconds for updates to settle.");
            Thread.Sleep(20000);

            // Confirm update with tx through local parity
            _cw.ConfirmUpdate().Wait();
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

        private void UpdateChainSpec(StateChangeAction act)
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
            _dcc.ApplyChangesToStack(_stackPath, true);
        }

        private void UpdateDocker(StateChangeAction act, ExpectedNodeState expectedState)
        {
            // pull docker image
            DockerClient client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
                .CreateClient();

            // TODO: verify its a proper docker image tag
            Log($"Pulling new parity image [{act.Payload}] ..");


            string msg = string.Empty;
            Progress<JSONMessage> progress = new Progress<JSONMessage>();
            progress.ProgressChanged += (sender, message) =>
            {
                string newMsg = $"[DOCKER IMAGE PULL | {message.ID}] {message.Status}...";
                if (newMsg != msg)
                {
                    Log(newMsg);
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
                Log("Image hashes don't match. Cancel update.");
                client.Images.DeleteImageAsync(act.Payload, new ImageDeleteParameters()).Wait();
                Log("Pulled imaged removed.");
                throw new UpdateVerificationException("Docker image hashes don't match.");
            }

            // Image is legit. update docker compose
            Log("Image valid. Updating stack.");

            // modify docker-compose env file
            _configProvider.WriteNewState(expectedState);

            // restart/upgrade stack
            _dcc.ApplyChangesToStack(_stackPath, false);
        }
    }
}