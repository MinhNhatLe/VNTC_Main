using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Phải nhập địa chỉ email hoặc tên tài khoản")]
        [EmailAddress(ErrorMessage = "Sai định dạng Email")]
        public string Email { get; set; }
    }
}
