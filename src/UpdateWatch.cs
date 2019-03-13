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
    /// <summary>
    /// Watches for new updates, compares the current and expected state and carries out the state changes if nessesary
    /// </summary>
    public class UpdateWatch
    {
        
        /// <summary>
        /// Event that gets triggered on a new log message. Use to get log output from UpdateWatch
        /// </summary>
        internal event EventHandler<LogEventArgs> OnLog
        {
            add => _onLog += value;
            // TODO: Rider complains about delegate subtraction - find better way - https://www.jetbrains.com/help/rider/DelegateSubtraction.html
            remove => _onLog -= value;
        }

        /// <summary>
        /// Event handler for log messages
        /// </summary>
        private EventHandler<LogEventArgs> _onLog;

        /// <summary>
        /// ContractWrapper implementation given by the constructor options
        /// </summary>
        private readonly IContractWrapper _cw;
        
        /// <summary>
        /// state compare instance initialized with the configuration provider given in construction options
        /// </summary>
        private readonly StateCompare _sc;
        
        /// <summary>
        /// Message service implementation given via the constructor options
        /// </summary>
        private readonly IMessageService _msgService;
        
        /// <summary>
        /// Path to the docker-compose stack given by the constructor options 
        /// </summary>
        private readonly string _stackPath;
        
        /// <summary>
        /// ConfigurationProvider implementation given by the constructor options
        /// </summary>
        private readonly IConfigurationProvider _configProvider;
        
        /// <summary>
        /// DockerComposeControl implementation given by the constructor options
        /// </summary>
        private readonly IDockerComposeControl _dcc;

        /// <summary>
        /// Create a new instance of UpdateWatch.
        /// </summary>
        /// <param name="opts">Filled options object to configure update watch</param>
        public UpdateWatch(UpdateWatchOptions opts)
        {
            _cw = new ContractWrapper(opts.ContractAddress, opts.RpcEndpoint, opts.ValidatorAddress);
            _sc = new StateCompare(opts.ConfigurationProvider);
            _msgService = opts.MessageService;
            _stackPath = opts.DockerStackPath;
            _configProvider = opts.ConfigurationProvider;
            _dcc = opts.DockerComposeControl;
        }


        /// <summary>
        /// Start a timer that will periodically check for new updates
        /// </summary>
        public void StartWatch()
        {
            Log("Starting watch");
            Timer checkTimer = new Timer(CheckForUpdates);
            checkTimer.Change(10000, 10000);
        }

        /// <summary>
        /// Abstract log method to log arbitrary messages. Fires the OnLog() event.
        /// </summary>
        /// <param name="message">The message that should be send to the log event</param>
        private void Log(string message)
        {
            _onLog(this, new LogEventArgs(message));
        }

        /// <summary>
        /// Method called by the timer to check and execute updates
        /// </summary>
        /// <param name="state">dummy state that is not used</param>
        /// <remarks>Errors during processing will not throw exceptions, but instead send a message via the message service.</remarks>
        private void CheckForUpdates(object state)
        {
            Log("Checking On-Chain for updates.");
            
            if(!_cw.HasNewUpdate().Result)
            {
                // No new update events on chain
                return;
            }
                
            // Query block chain to receive expectedcdd state
            NodeState expectedState = _cw.GetExpectedState().Result;

            // calculate actions from state difference 
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
                        case UpdateMode.Docker: // Update docker image
                            UpdateDocker(act, expectedState);
                            break;
                        case UpdateMode.ChainSpec: // Update chainspec
                            UpdateChainSpec(act);
                            break;
                        default:
                            throw new UpdateVerificationException("Unsupported update mode.");
                    }
                }
                catch (UpdateVerificationException uve)
                {
                    _msgService.SendErrorMessage("Unable to verify update", uve.Message, expectedState);
                }
                catch (Exception ex)
                {
                    _msgService.SendErrorMessage("Unknown error during update", ex.Message, expectedState);
                }
            }

            // wait until parity is back online
            Log("Waiting 20 seconds for updates to settle.");
            Thread.Sleep(20000);

            // Confirm update with tx through local parity
            _cw.ConfirmUpdate().Wait();
        }

        /// <summary>
        /// Hash string with SHA256 and return has as hex string
        /// </summary>
        /// <param name="dataToHash">Data that should be hashed</param>
        /// <returns>hash as hex encoded string</returns>
        private static string HashString(string dataToHash)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                foreach (byte t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }

                return builder.ToString();
            }
        }
        
        
        /// <summary>
        /// Downloads, verifies and updates the chain spec file
        /// </summary>
        /// <param name="act">The action containing the new chainspec url and checksum</param>
        /// <exception cref="UpdateVerificationException">
        /// Thrown when update is not able to verify. This can happen when:
        /// <list type="bullet">
        /// <item>URL to chain file is not https://</item>
        /// <item>Checksum of the downloaded content doesn't match</item>
        /// <item>Unable to read the current chainspec file. Maybe someone messed with the file.</item>
        /// </list>
        /// </exception>
        
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

        /// <summary>
        /// Pulland verify a new docker image and update the docker-compose file 
        /// </summary>
        /// <param name="act">The action containing the new chainspec url and checksum</param>
        /// <param name="expectedState">The expected state that the action was derived from</param>
        /// <exception cref="UpdateVerificationException">
        /// Thrown when update is not able to be verified. Reasons:
        /// <list type="bullet">
        /// <item>The image is not able to be pulled</item>
        /// <item>Checksum of the downloaded content doesn't match</item>
        /// </list>
        /// </exception>
        private void UpdateDocker(StateChangeAction act, NodeState expectedState)
        {
            // Connect to local docker deamon
            DockerClient client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
                .CreateClient();

            Log($"Pulling new parity image [{act.Payload}] ..");

            // Prepare progress logging
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

            try
            {
                // pull docker image
                client.Images.CreateImageAsync(new ImagesCreateParameters
                {
                    FromImage = act.Payload.Split(':')[0],
                    Tag = act.Payload.Split(':')[1]
                }, null, progress).Wait();
            }
            catch (Exception e)
            {
                throw new UpdateVerificationException("Unable to pull new image.",e);
            }

            // verify docker image id against expected hash
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