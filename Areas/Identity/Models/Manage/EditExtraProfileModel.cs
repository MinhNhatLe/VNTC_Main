using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.ManageViewModels
{
    public class EditExtraProfileModel
    {
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(150, ErrorMessage = "Vui lòng nhập địa chỉ từ 5 đến 150 kí tự", MinimumLength = 5)]
        public string HomeAdress { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}