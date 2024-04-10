using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.UserViewModels
{
    public class SetUserPasswordModel
    {
        [Required(ErrorMessage = "Phải nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải dài trên 5 ký tự.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Lặp lại mật khẩu không chính xác.")]
        [Required(ErrorMessage = "Phải xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
    }
}