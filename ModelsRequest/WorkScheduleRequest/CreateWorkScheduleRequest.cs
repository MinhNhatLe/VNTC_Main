using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.ModelsRequest.WorkScheduleRequest
{
    public class CreateWorkScheduleRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên lịch")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Vui lòng nhập tên lịch từ 3 đến 200 kí tự")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Note { get; set; }

        public string? Address { get; set; }

        public string? Participants { get; set; }

        public DateTime? ActionDate { get; set; }
    }
}
