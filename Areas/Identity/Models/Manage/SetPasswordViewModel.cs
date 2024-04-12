using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(70, ErrorMessage = "Vui lòng nhập tên mật khẩu mới từ 5 đến 70 kí tự", MinimumLength = 5)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu lặp lại không chính xác")]
        public string ConfirmPassword { get; set; }
    }
}
