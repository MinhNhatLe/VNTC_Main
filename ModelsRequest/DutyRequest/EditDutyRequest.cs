﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnetstartermvc.ModelsRequest.DutyRequest
{
    public class EditDutyRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nhiệm vụ")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Vui lòng nhập tên nhiệm vụ từ 3 đến 200 kí tự")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Note { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đăng")]
        public DateTime? ActionDate { get; set; }

        public bool IsComplete { get; set; }

        public string[]? Emails { get; set; }
    }
}
