using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "Phải nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải dài trên 5 ký tự..", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận phải trùng với mật khẩu mới")]
        [Required(ErrorMessage = "Phải xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
    }
}
