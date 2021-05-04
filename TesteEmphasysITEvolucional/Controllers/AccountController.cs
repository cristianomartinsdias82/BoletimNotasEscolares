using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Services.Security.Authentication;
using TesteEmphasysITEvolucional.ViewModels;

namespace TesteEmphasysITEvolucional.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public  IActionResult Login(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var authenticationResult = await _authenticationService.SignInAsync(model.Username, model.Password, returnUrl, HttpContext, cancellationToken);
            if (!authenticationResult.Successful || !authenticationResult.Data)
            {
                ModelState.AddModelError(string.Empty, authenticationResult.Message);
                return View(model);
            }

            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            await _authenticationService.SignOutAsync(HttpContext, cancellationToken);

            return Redirect("~/");
        }
    }
}
