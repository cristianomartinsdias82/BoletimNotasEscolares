using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TesteEmphasysITEvolucional.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("User name")]
        [Required]
        public string Username { get; set; }

        [DisplayName("Password")]
        [Required]
        public string Password { get; set; }
    }
}
