using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.ModelsRequest.NotificationRequest
{
    public class EditNotificationRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tin tức")]
        [StringLength(80, ErrorMessage = "Tên tin tức không được vượt quá 80 kí tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung tin tức")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đăng")]
        public DateTime CreatedDate { get; set; }
    }
}
