using System.Threading.Tasks;
using src.Models;

namespace src.Interfaces
{
    public interface IContractWrapper
    {
        Task<bool> HasNewUpdate();
        Task<ExpectedNodeState> GetExpectedState();
        Task ConfirmUpdate();
    }
}