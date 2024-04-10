using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.RoleViewModels
{
    public class CreateRoleModel
    {
        [Required(ErrorMessage = "Phải nhập tên của role")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Tên của vai trò phải dài trên 3 ký tự")]
        public string Name { get; set; }
    }
}
