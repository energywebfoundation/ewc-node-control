using System;
using System.Diagnostics;
using System.IO;
using src.Interfaces;

namespace src
{
    public class LinuxComposeControl : IDockerComposeControl
    {

        public void ApplyChangesToStack(string pathToStack, bool restartOnly)
        {

            if (!Directory.Exists(pathToStack))
            {
                throw new DirectoryNotFoundException("docker stack directory not found.");
            }

            string cmd = restartOnly ? "restart" : "up -d";
            
            Console.WriteLine("Apply changes to compose stack...");
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
    }
}