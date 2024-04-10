using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.RoleViewModels
{
    public class EditRoleModel
    {
        [Required(ErrorMessage = "Phải nhập tên của role")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Tên của vai trò phải dài trên 3 ký tự")]
        public string Name { get; set; }

        public List<IdentityRoleClaim<string>> Claims { get; set; }

        public IdentityRole role { get; set; }
    }
}
