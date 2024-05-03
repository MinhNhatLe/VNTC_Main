using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.ModelsRequest.RecruitmentRequest
{
    public class CreateRecruitmentRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên công việc")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Vui lòng nhập tên công việc từ 3 đến 200 kí tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, 100, ErrorMessage = "Vui lòng nhập số lượng từ 1 đến 100.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung công việc")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đăng")]
        public DateTime CreatedDate { get; set; }
    }
}
