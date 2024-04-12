using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Areas.Identity.Models.ManageViewModels
{
    public class EditExtraProfileModel
    {
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(15, ErrorMessage = "Vui lòng nhập tên tài khoản không quá 15 kí tự")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(30, ErrorMessage = "Vui lòng nhập tên tài khoản không quá 30 kí tự")]
        public string Lastname { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(150, ErrorMessage = "Vui lòng nhập địa chỉ từ 5 đến 150 kí tự", MinimumLength = 5)]
        public string HomeAdress { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}