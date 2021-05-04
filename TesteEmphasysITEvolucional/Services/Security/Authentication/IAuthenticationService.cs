using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Common;

namespace TesteEmphasysITEvolucional.Services.Security.Authentication
{
    public interface IAuthenticationService
    {
        Task<OperationResult<bool>> SignInAsync(string username, string password, string redirectUrl, HttpContext httpContext, CancellationToken cancellationToken);
        Task SignOutAsync(HttpContext httpContext, CancellationToken cancellationToken);
    }
}
