using System;
using System.Collections.Generic;
using System.IO;
using src.Interfaces;
using src.Models;

namespace src
{
    public class ConfigurationFileHandler : IConfigurationProvider
    {
        private readonly string _envFile;

        // environment variable mapping
        private const string ParityVersion = "PARITY_VERSION";
        private const string ParityChksum = "PARITY_CHKSUM";
        private const string ChainspecChksum = "CHAINSPEC_CHKSUM";
        private const string ChainspecUrl = "CHAINSPEC_URL";
        private const string IsSigning = "IS_SIGNING";
        
        public ConfigurationFileHandler(string pathToEnvFile)
        {
            if (string.IsNullOrWhiteSpace(pathToEnvFile))
            {
                throw new ArgumentException("No path given.");
            }

            if (!File.Exists(pathToEnvFile))
            {
                throw new FileNotFoundException("No file found at path.");
            }

            _envFile = pathToEnvFile;
        }

        public NodeState ReadCurrentState()
        {
            if (!File.Exists(_envFile))
            {
                throw new FileNotFoundException("Settings file disappeared");
            }

            NodeState state = new NodeState();
            foreach (string line in File.ReadAllLines(_envFile))
            {
                string[] kv = line.Split('=');
                if (kv.Length != 2)
                {
                    // Not a line we can parse. Skip
                    continue;
                }

                switch (kv[0])
                {
                    case ParityVersion:
                        state.DockerImage = kv[1];
                        break;
                    case ParityChksum:
                        state.DockerChecksum = kv[1];
                        break;
                    case ChainspecChksum:
                        state.ChainspecChecksum = kv[1];
                        break;
                    case ChainspecUrl:
                        state.ChainspecUrl = kv[1];
                        break;
                    case IsSigning:
                        state.IsSigning = kv[1] == "1";
                        break;
                }
            }

            return state;
        }

        public void WriteNewState(NodeState newState)
        {
            if (!File.Exists(_envFile))
            {
                throw new FileNotFoundException("Settings file disappeared");
            }

            List<string> newFileContents = new List<string>();

            foreach (string line in File.ReadAllLines(_envFile))
            {
                // replace any value we have authority over with the state value
                if (line.StartsWith(ParityVersion) && !string.IsNullOrWhiteSpace(newState.DockerImage))
                {
                    newFileContents.Add($"{ParityVersion}={newState.DockerImage}");
                }
                else if (line.StartsWith(ParityChksum)&& !string.IsNullOrWhiteSpace(newState.DockerChecksum))
                {
                    newFileContents.Add($"{ParityChksum}={newState.DockerChecksum}");
                }
                else if (line.StartsWith(ChainspecChksum)&& !string.IsNullOrWhiteSpace(newState.ChainspecChecksum))
                {
                    newFileContents.Add($"{ChainspecChksum}={newState.ChainspecChecksum}");
                }
                else if (line.StartsWith(ChainspecUrl)&& !string.IsNullOrWhiteSpace(newState.ChainspecUrl))
                {
                    newFileContents.Add($"{ChainspecUrl}={newState.ChainspecUrl}");
                }
                else if (line.StartsWith(IsSigning))
                {
                    string signing = newState.IsSigning ? "1" : "0";
                    newFileContents.Add($"{IsSigning}={signing}");
                }
                else
                {
                    newFileContents.Add(line);
                }
            }

            // Write changed config to disk
            File.WriteAllLines(_envFile, newFileContents);
        }
    }
}