using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.ModelsRequest.ServicesRequest
{
    public class EditServicesRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ")]
        [StringLength(80, ErrorMessage = "Tên dịch vụ không được vượt quá 80 kí tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung dịch vụ")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đăng")]
        public DateTime CreatedDate { get; set; }
    }
}
