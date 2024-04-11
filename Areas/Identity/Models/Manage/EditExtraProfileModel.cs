using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.ManageViewModels
{
    public class EditExtraProfileModel
    {
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Phải nhập tên")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Phải nhập họ")]
        public string Lastname { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Phải nhập địa chỉ")]
        [StringLength(400)]
        public string HomeAdress { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}