using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.ModelsRequest.ServicesRequest
{
    public class CreateServicesRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Vui lòng nhập tên dịch vụ từ 3 đến 200 kí tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung dịch vụ")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đăng")]
        public DateTime CreatedDate { get; set; }
    }
}
