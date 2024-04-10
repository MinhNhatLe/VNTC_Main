using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Phải nhập tên tài khoản")]
        [StringLength(100, ErrorMessage = "Tài khoản phải trên 5 ký tự.", MinimumLength = 5)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Phải nhập email")]
        [EmailAddress(ErrorMessage = "Sai định dạng Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phải nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải trên 5 ký tự", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu", Prompt = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác.")]
        [Required(ErrorMessage = "Phải nhập mật khẩu")]
        public string ConfirmPassword { get; set; }
    }
}
