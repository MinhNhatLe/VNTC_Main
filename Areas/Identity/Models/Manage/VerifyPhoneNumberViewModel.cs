using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.ManageViewModels
{
    public class VerifyPhoneNumberViewModel
    {
        [Display(Name = "Mã xác nhận")]
        [Required(ErrorMessage = "Vui lòng nhập mã xác nhận")]
        public string Code { get; set; }

        [Phone]
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 số")]
        public string PhoneNumber { get; set; }
    }
}
