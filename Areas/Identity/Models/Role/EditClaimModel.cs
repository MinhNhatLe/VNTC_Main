using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.RoleViewModels
{
    public class EditClaimModel
    {
        [Required(ErrorMessage = "Vui lòng nhập kiểu (tên) claim")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Vui lòng nhập kiểu (tên) claim từ 3 đến 50 kí tự")]
        public string ClaimType { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Vui lòng nhập giá trị từ 3 đến 50 kí tự")]
        public string ClaimValue { get; set; }

        public IdentityRole role { get; set; }
    }
}
