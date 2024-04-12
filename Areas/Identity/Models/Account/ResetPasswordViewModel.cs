using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [StringLength(70, ErrorMessage = "Vui lòng nhập địa chỉ email không được vượt quá 70 kí tự.")]
        [EmailAddress(ErrorMessage = "Phải đúng định dạng email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(70, ErrorMessage = "Vui lòng nhập mật khẩu mới từ 5 đến 70 kí tự", MinimumLength = 5)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phải nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
