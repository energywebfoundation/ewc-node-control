using System;
using System.Threading;

namespace src
{
    class Program
    {
        private static readonly CheckState _checkState = new CheckState();
        static void Main(string[] args)
        {
            Console.WriteLine("EWF NodeControl");

            int checkIntervalSeconds = 60;
            
            while (true)
            {
                Console.WriteLine("Checking On-Chain for updates...");
                Thread.Sleep(checkIntervalSeconds * 1000);
            }
     
        }

        
    }
}
