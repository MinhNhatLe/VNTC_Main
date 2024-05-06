using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Models
{
    public class Duty : BaseEntity
    {
        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Note { get; set; }

        public string? Feedback { get; set; }

        public bool IsComplete { get; set; }

        public DateTime? ActionDate { get; set; }

        [Required]
        public DateTime? CreatedDate { get; set; }

        [Required]
        public DateTime? UpdatedDate { get; set; }

        public List<AppUser>? Users { get; set; }
    }
}
