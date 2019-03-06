using System;
using System.Diagnostics;
using System.IO;

namespace src
{
    public static class DockerControl
    {

 
        public static void ApplyChangesToStack(string pathToStack)
        {

            if (!Directory.Exists(pathToStack))
            {
                throw new DirectoryNotFoundException("docker stack directory not found.");
            }
            
            Console.WriteLine("Apply changes to compose stack...");
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = "/usr/bin/docker-compose";
                myProcess.StartInfo.WorkingDirectory = pathToStack;
                myProcess.StartInfo.Arguments = "up -d";
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