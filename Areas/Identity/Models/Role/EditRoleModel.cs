using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.RoleViewModels
{
    public class EditRoleModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên của vai trò")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Vui lòng nhập tên của vai trò từ 3 đến 50 kí tự")]
        public string Name { get; set; }

        public List<IdentityRoleClaim<string>> Claims { get; set; }

        public IdentityRole role { get; set; }
    }
}
