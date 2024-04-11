using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.ModelsRequest.RecruitmentRequest
{
    public class EditRecruitmentRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên công việc")]
        [StringLength(80, ErrorMessage = "Tên công việc không được vượt quá 80 kí tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung công việc")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đăng")]
        public DateTime CreatedDate { get; set; }
    }
}
