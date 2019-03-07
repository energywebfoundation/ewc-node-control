using System.Threading.Tasks;

namespace src.Contract
{
    public interface IContractWrapper
    {
        Task<ExpectedNodeState> GetExpectedState();
        Task ConfirmUpdate();
    }
}