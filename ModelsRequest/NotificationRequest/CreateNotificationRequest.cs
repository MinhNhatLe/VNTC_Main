﻿using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.ModelsRequest.NotificationRequest
{
    public class CreateNotificationRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tin tức")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Vui lòng nhập tên tin tức từ 3 đến 200 kí tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung tin tức")]
        public string Description { get; set; }
    }
}
