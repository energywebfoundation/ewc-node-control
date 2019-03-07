using System.Collections.Generic;
using System.IO;
using src.Interfaces;
using src.Models;

namespace src
{
    public class ConfigurationFileHandler : IConfigurationProvider
    {

        private readonly string _envFile;

        public ConfigurationFileHandler(string pathToEnvFile)
        {
            _envFile = pathToEnvFile;
        }

        public ExpectedNodeState ReadCurrentState()
        {
            ExpectedNodeState state = new ExpectedNodeState();
            foreach (string line in File.ReadAllLines(_envFile))
            {
                string[] kv = line.Split('=');
                switch (kv[0])
                {
                    case "PARITY_VERSION":
                        state.DockerImage = kv[1];
                        break;
                    case "PARITY_CHKSUM":
                        state.DockerChecksum = kv[1];
                        break;
                    case "CHAINSPEC_CHKSUM":
                        state.ChainspecChecksum = kv[1];
                        break;
                    case "IS_SIGNING":
                        state.IsSigning= kv[1] == "1";
                        break;
                }
            }

            return state;
        }
        
        public void WriteNewState(ExpectedNodeState newState)
        {
            if (!File.Exists(_envFile))
            {
                throw new FileNotFoundException("Settings file disappeared");
            }

            List<string> newFileContents = new List<string>();
            foreach (string line in File.ReadAllLines(_envFile))
            {
                // replace any value we have authority over with the state value
                if (line.StartsWith("PARITY_VERSION"))
                {
                    newFileContents.Add($"PARITY_VERSION={newState.DockerImage}");
                } 
                else if (line.StartsWith("PARITY_CHKSUM"))
                {
                    newFileContents.Add($"PARITY_CHKSUM={newState.DockerChecksum}");
                } 
                else if (line.StartsWith("CHAINSPEC_CHKSUM"))
                {
                    newFileContents.Add($"CHAINSPEC_CHKSUM={newState.ChainspecChecksum}");
                }
                else if (line.StartsWith("IS_SIGNING"))
                {
                    string signing = newState.IsSigning ? "1" : "0";
                    newFileContents.Add($"IS_SIGNING={signing}");
                }
                else
                {
                    newFileContents.Add(line);
                }
            }
                
            
            // Write changed config to disk
            File.WriteAllLines(_envFile,newFileContents);
        }
        
    }
}