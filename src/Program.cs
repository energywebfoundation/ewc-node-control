using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Docker.DotNet.Models;
using Nethereum.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            // Read key password
            string secretPath = Path.Combine(watchOpts.DockerStackPath, ".secret");
            if (!File.Exists(secretPath))
            {
                logger.Log("Unable to read secret. Exiting.");
                return;
            }

            string keyPw = File.ReadAllText(secretPath).TrimEnd('\r','\n');
            
            string encKey;
            // Read the encrpyted key from parity
            using (HttpClient hc = new HttpClient())
            {
                string request =
                    $"{{ \"method\": \"parity_exportAccount\", \"params\": [\"{watchOpts.ValidatorAddress}\",\"{keyPw}\"], \"id\": 1, \"jsonrpc\": \"2.0\" }}";
                var sc = new StringContent(request,Encoding.UTF8,"application/json");
                var response = hc.PostAsync(watchOpts.RpcEndpoint, sc).Result;
                var resContent = response.Content.ReadAsStringAsync().Result;
                JObject resObj = JObject.Parse(resContent);

                if (resObj["result"] == null)
                {
                    // unable to export key
                    logger.Log($"Unable to retrieve key from parity. Exiting. ==> Response from parity: {resContent}");
                    return;
                }
                
                encKey = resObj["result"].ToString();
            }
                
            
            // Add dependencies
            watchOpts.ConfigurationProvider = new ConfigurationFileHandler(Path.Combine(watchOpts.DockerStackPath, ".env"));
            watchOpts.DockerControl = new LinuxDockerControl(logger);
            watchOpts.ContractWrapper = new ContractWrapper(watchOpts.ContractAddress,watchOpts.RpcEndpoint,watchOpts.ValidatorAddress,logger,keyPw,encKey, watchOpts.BlockNumberPersistFile);

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