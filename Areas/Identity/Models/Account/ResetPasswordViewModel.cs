using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Phải nhập đúng định dạng email")]
        [EmailAddress(ErrorMessage = "Phải đúng định dạng email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phải nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải dài trên 5 ký tự.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phải nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
