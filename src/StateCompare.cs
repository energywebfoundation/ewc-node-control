using System;
using System.Collections.Generic;
using src.Interfaces;
using src.Models;

namespace src
{
    public class StateCompare
    {
        private readonly IConfigurationProvider _configProvider;

        public StateCompare(IConfigurationProvider confProv)
        {
            _configProvider = confProv ?? throw new ArgumentNullException(nameof(confProv),"No Configuration provider given.");
        }

        public List<StateChangeAction> ComputeActionsFromState(ExpectedNodeState newState)
        {
            if (newState == null)
            {
                throw new ArgumentNullException(nameof(newState),"newState to compare can't be null");
            }
            
            ExpectedNodeState curState = _configProvider.ReadCurrentState();
            if (curState == null)
            {
                throw new StateCompareException("Received state from configuration provider is null. Can't compare");
            }
            
            // compare states and create according actions
            List<StateChangeAction> actions = new List<StateChangeAction>();
            
            // check for parity update
            if (curState.DockerChecksum != newState.DockerChecksum && curState.DockerImage != newState.DockerImage)
            {
                actions.Add(new StateChangeAction
                {
                    Mode = UpdateMode.Docker,
                    Payload = newState.DockerImage,
                    PayloadHash = newState.DockerChecksum
                });
            }
            
            // Check for chain spec change
            if (curState.ChainspecChecksum != newState.ChainspecChecksum && curState.ChainspecUrl != newState.ChainspecUrl)
            {
                actions.Add(new StateChangeAction
                {
                    Mode = UpdateMode.ChainSpec,
                    Payload = newState.ChainspecUrl,
                    PayloadHash = newState.ChainspecChecksum
                });
            }
            
            if (curState.IsSigning != newState.IsSigning)
            {
                actions.Add(new StateChangeAction
                {
                    Mode = UpdateMode.ToggleSigning,
                    Payload = newState.IsSigning.ToString(),
                    PayloadHash = String.Empty
                });
            }
            
            return actions;
        }
    }
}