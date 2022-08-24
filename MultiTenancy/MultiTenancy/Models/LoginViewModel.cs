using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Models {
    public class LoginViewModel {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo debe de ser un correo electronico valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "EL campo {0} es obligatorio")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}