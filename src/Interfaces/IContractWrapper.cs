using System.Threading.Tasks;
using src.Models;

namespace src.Interfaces
{
    public interface IContractWrapper
    {
        Task<ExpectedNodeState> GetExpectedState();
        Task ConfirmUpdate();
    }
}