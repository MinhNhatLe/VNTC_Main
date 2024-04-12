using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        [StringLength(70, ErrorMessage = "Vui lòng nhập tên tài khoản từ 5 đến 70 kí tự", MinimumLength = 5)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [StringLength(70, ErrorMessage = "Vui lòng nhập địa chỉ email không được vượt quá 70 kí tự.")]
        [EmailAddress(ErrorMessage = "Phải đúng định dạng email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(70, ErrorMessage = "Vui lòng nhập tên mật khẩu từ 5 đến 70 kí tự", MinimumLength = 5)]
        [Display(Name = "Mật khẩu", Prompt = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác")]
        public string ConfirmPassword { get; set; }
    }
}
