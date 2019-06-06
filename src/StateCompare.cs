using System;
using System.Collections.Generic;
using src.Interfaces;
using src.Models;

namespace src
{
    /// <summary>
    /// Compares node states and computes any necessary actions
    /// </summary>
    public class StateCompare
    {
        /// <summary>
        /// Configuration provider to read the current state from
        /// </summary>
        private readonly IConfigurationProvider _configProvider;

        /// <summary>
        /// Instantiate a new comparer
        /// </summary>
        /// <param name="confProv">configuration provider used to read the current state</param>
        public StateCompare(IConfigurationProvider confProv)
        {
            _configProvider = confProv ?? throw new ArgumentNullException(nameof(confProv),"No Configuration provider given.");
        }

        /// <summary>
        /// Compares the given state against the state read from the config provider
        /// </summary>
        /// <param name="newState">State that should be reached</param>
        /// <returns>List of action needed to transition from the current state to the new state</returns>
        /// <exception cref="ArgumentNullException">Thrown when the given state is null</exception>
        /// <exception cref="StateCompareException">Thrown when the current state can't be read from the config provider</exception>
        public List<StateChangeAction> ComputeActionsFromState(NodeState newState)
        {
            if (newState == null)
            {
                throw new ArgumentNullException(nameof(newState),"newState to compare can't be null");
            }

            NodeState curState = _configProvider.ReadCurrentState();
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

            // Check for signing change
            if (curState.IsSigning != newState.IsSigning)
            {
                actions.Add(new StateChangeAction
                {
                    Mode = UpdateMode.ToggleSigning,
                    Payload = newState.IsSigning.ToString(),
                    PayloadHash = string.Empty
                });
            }

            return actions;
        }
    }
}