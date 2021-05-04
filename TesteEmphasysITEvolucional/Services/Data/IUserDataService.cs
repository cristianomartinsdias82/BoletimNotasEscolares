using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Common;

namespace TesteEmphasysITEvolucional.Services.Data
{
    public interface IUserDataService
    {
        Task<OperationResult<bool>> CheckEnteredCredentialsAsync(string username, string password, CancellationToken cancellationToken);
    }
}
