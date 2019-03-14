using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using src.Interfaces;

namespace src
{
    /// <summary>
    /// Implements docker compose control using linux shell commands
    /// </summary>
    public class LinuxComposeControl : IDockerComposeControl
    {

        /// <summary>
        /// Use the docker-compose CLI to apply changes 
        /// </summary>
        /// <param name="pathToStack">Path to the docker-compose directory</param>
        /// <param name="restartOnly">Should the stack only restart</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the given directory doesn't exist</exception>
        public void ApplyChangesToStack(string pathToStack, bool restartOnly)
        {

            // Check directory
            if (!Directory.Exists(pathToStack))
            {
                throw new DirectoryNotFoundException("docker stack directory not found.");
            }

            // Decide on compose parameters
            string cmd = restartOnly ? "restart" : "up -d";
            
            Console.WriteLine("Apply changes to compose stack...");
            
            // Fire up a new shell process to apply the changes
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = "/usr/bin/docker-compose";
                myProcess.StartInfo.WorkingDirectory = pathToStack;
                myProcess.StartInfo.Arguments = cmd;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.Start();

                while (!myProcess.StandardOutput.EndOfStream)
                {
                    string composeLine = myProcess.StandardOutput.ReadLine();
                    Console.WriteLine("COMPOSE: " + composeLine);
                }
            }

            Console.WriteLine("Done.");
        }

        public void PullImage(ImagesCreateParameters imagesCreateParameters, AuthConfig authConfig, Progress<JSONMessage> progress)
        {
            // Connect to local docker deamon
            using (DockerClient client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient())
            {
                client.Images.CreateImageAsync(imagesCreateParameters, authConfig, progress).Wait();
            }
        }

        public ImageInspectResponse InspectImage(string dockerImage)
        {
            // Connect to local docker deamon
            using (DockerClient client =
                new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient())
            {
                return client.Images.InspectImageAsync(dockerImage).Result;    
            }
            
        }

        public void DeleteImage(string dockerImage)
        {
            using (DockerClient client =
                new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient())
            {
                client.Images.DeleteImageAsync(dockerImage, new ImageDeleteParameters()).Wait();    
            }
        }
    }
}