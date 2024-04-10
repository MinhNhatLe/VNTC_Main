using Microsoft.AspNetCore.Identity;

namespace dotnetstartermvc.Areas.Identity.Models.RoleViewModels
{
    public class RoleModel : IdentityRole
    {
        public string[] Claims { get; set; }

    }
}
