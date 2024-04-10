using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.ManageViewModels
{
    public class AddPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Phải nhập số điện thoại")]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
