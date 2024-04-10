using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Phải nhập địa chỉ email hoặc tên tài khoản")]
        public string UserNameOrEmail { get; set; }

        [Required(ErrorMessage = "Phải nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Nhớ thông tin đăng nhập?")]
        public bool RememberMe { get; set; }
    }
}
