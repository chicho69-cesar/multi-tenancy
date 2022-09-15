using MultiTenancy.Validations;
using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Entities {
    public enum Permissions {
        [Hide]
        Null = 0, // Permiso que todos los usuarios tienen por ser miembros de una empresa. Solo se elimina al desvincular a un usuario de una empresa.
        [Display(Description = "Puede crear productos")]
        Products_Create = 1,
        [Display(Description = "Puede leer productos")]
        Product_Read = 2,
        User_Vinculate = 3,
        Permissions_Read = 4,
        Permissions_Update = 5,
    }
}