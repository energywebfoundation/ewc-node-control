using System;
using System.Threading;

namespace src
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("EWF NodeControl");
            
     
        }
    }

    public class UpdateChecker
    {
        private int _interval;
        private UpdateState _lastCheckState;
        private Timer _timer;

        public UpdateChecker(int interval)
        {
            _interval = interval;
            _lastCheckState = new UpdateState
            {
                LastBlock = 0
                
            };
        }

        public void StartListening()
        {
            
            
            _timer = new Timer(
                callback: new TimerCallback(CheckForUpdate),
                state: _lastCheckState,
                dueTime: 1000,
                period: _interval * 1000
                );
            
        }

        private static void CheckForUpdate(object state)
        {
            // check for new block
            
            // check for event in block
            
            // if event then query contract
            
        }
        
    }

    public class UpdateState
    {
        public int LastBlock { get; set; }
    }

    public class DockerControl
    {

        private string _dockerSocket;
        
        public DockerControl(string sock)
        {
            _dockerSocket = sock;
        }


        public void UpdateContainer(string name, string newImage)
        {
            
        }
    }
}
