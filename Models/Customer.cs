using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Models
{
    public class Customer : BaseEntity
    {
        [Required]
        public string CompanyName { get; set; }

        public string? Address { get; set; }

        public string? ProvinceOrCity { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 số")]
        public string? CompanyPhoneNumber { get; set; }



        [Required]
        public string? ContactPersonName { get; set; }

        public string? Position { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 số")]
        public string? ContactPersonPhoneNumber { get; set; }

        public string? Email { get; set; }

        public List<AppUser>? Users { get; set; }




        [Required]
        public string? BidPackageName { get; set; }

        [Required]
        public string? ProjectValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? OpportunitySource { get; set; }

        public string? Notes { get; set; }

        public bool IsComplete { get; set; }

        public string? Feedback { get; set; }




        [Required]
        public DateTime? CreatedDate { get; set; }

        [Required]
        public DateTime? UpdatedDate { get; set; }

        [Required]
        public string UserId { get; set; }

        public List<BidPackage>? BidPackages { get; set; }
    }
}
