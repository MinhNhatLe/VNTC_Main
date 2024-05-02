using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnetstartermvc.Models
{
    public class Assignment : BaseEntity
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
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [Required]
        public AppUser? User { get; set; }
    }
}
