using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.ModelsRequest.CustomerRequest
{
    public class CreateCustomerRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên công ty")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Vui lòng nhập tên công ty từ 2 đến 200 kí tự")]
        public string CompanyName { get; set; }

        public string? Address { get; set; }

        public string? ProvinceOrCity { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 số")]
        public string? CompanyPhoneNumber { get; set; }



        [Required(ErrorMessage = "Vui lòng nhập tên người liên hệ")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Vui lòng nhập tên người liên hệ từ 1 đến 200 kí tự")]
        public string? ContactPersonName { get; set; }

        public string? Position { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 số")]
        public string? ContactPersonPhoneNumber { get; set; }

        public string? Email { get; set; }



        [Required(ErrorMessage = "Vui lòng nhập tên gói thầu")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Vui lòng nhập tên gói thầu từ 2 đến 200 kí tự")]
        public string? BidPackageName { get; set; }

        [Required(ErrorMessage = "Giá trị dự án")]
        public string? ProjectValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsComplete { get; set; }

        public string? OpportunitySource { get; set; }

        public string? Notes { get; set; }

        public string[]? Emails { get; set; }
    }
}
