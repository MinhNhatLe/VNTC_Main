using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email hoặc tên tài khoản")]
        [StringLength(70, ErrorMessage = "Vui lòng nhập địa chỉ email hoặc tên tài khoản không được vượt quá 70 kí tự.")]
        [EmailAddress(ErrorMessage = "Phải đúng định dạng email")]
        public string Email { get; set; }
    }
}
