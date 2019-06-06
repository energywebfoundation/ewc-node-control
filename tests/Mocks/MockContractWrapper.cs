using System;
using System.Threading.Tasks;
using src.Interfaces;
using src.Models;

namespace tests.Mocks
{
    public class MockContractWrapper : IContractWrapper
    {
        public Task<bool> HasNewUpdate()
        {
            throw new NotImplementedException();
        }

        public Task<NodeState> GetExpectedState()
        {
            throw new NotImplementedException();
        }

        public Task ConfirmUpdate()
        {
            throw new NotImplementedException();
        }
    }
}