using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.RoleViewModels
{
    public class EditClaimModel
    {
        [Required(ErrorMessage = "Phải nhập kiểu (tên) claim")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Kiểu (tên) claim phải dài trên 3 ký tự")]
        public string ClaimType { get; set; }

        [Required(ErrorMessage = "Phải nhập giá trị")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Giá trị phải dài trên 3 ký tự")]
        public string ClaimValue { get; set; }

        public IdentityRole role { get; set; }
    }
}
