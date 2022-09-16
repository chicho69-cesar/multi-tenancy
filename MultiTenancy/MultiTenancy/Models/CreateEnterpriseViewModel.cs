using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Models {
    public class CreateEnterpriseViewModel {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombre")]
        public string Name { get; set; }
    }
}