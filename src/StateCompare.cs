using System;
using System.Collections.Generic;

namespace src
{
    public class StateCompare
    {
        private readonly IConfigurationProvider _configProvider;

        public StateCompare(IConfigurationProvider confProv)
        {
            _configProvider = confProv ?? throw new ArgumentException("No Configuration provider given.", nameof(confProv));
        }

        public List<StateChangeAction> ComputeActionsFromState(ExpectedNodeState newState)
        {
            var curState = _configProvider.ReadCurrentState();
            List<StateChangeAction> actions = new List<StateChangeAction>();

            // compare states and create according actions
            
            // check for parity update
            if (curState.DockerChecksum != newState.DockerChecksum)
            {
                actions.Add(new StateChangeAction
                {
                    Mode = UpdateMode.Docker,
                    Payload = newState.DockerImage,
                    PaylodSignature = newState.DockerChecksum
                });
            }
            
            if (curState.ChainspecChecksum != newState.ChainspecChecksum)
            {
                actions.Add(new StateChangeAction
                {
                    Mode = UpdateMode.ChainSpec,
                    Payload = newState.ChainspecUrl,
                    PaylodSignature = newState.ChainspecChecksum
                });
            }
            
            return actions;
        }
    }
}