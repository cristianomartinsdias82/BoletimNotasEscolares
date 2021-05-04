using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Common;
using TesteEmphasysITEvolucional.Services.Data;

namespace TesteEmphasysITEvolucional.Services.Security.Authentication
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        public readonly IUserDataService _userDataService;

        public AuthenticationService(IUserDataService userDataService)
        {
            _userDataService = userDataService ?? throw new ArgumentNullException("User repository argument cannot be null.");
        }

        public async Task<OperationResult<bool>> SignInAsync(string username, string password, string redirectUrl, HttpContext httpContext, CancellationToken cancellationToken)
        {
            var authenticationResult = await _userDataService.CheckEnteredCredentialsAsync(username, password, cancellationToken);
            if (authenticationResult.Successful && authenticationResult.Data)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Administrators")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var now = DateTimeOffset.UtcNow;
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = now.AddMinutes(30),
                    IsPersistent = true,
                    IssuedUtc = now,
                    RedirectUri = redirectUrl
                };

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }

            return authenticationResult;
        }

        public async Task SignOutAsync(HttpContext httpContext, CancellationToken cancellationToken)
            => await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}