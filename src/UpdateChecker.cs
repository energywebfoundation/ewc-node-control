using System.Threading;

namespace src
{
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
}