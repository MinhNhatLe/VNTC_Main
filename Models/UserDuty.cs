using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Models
{
    public class UserDuty
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public AppUser? User { get; set; }

        [Required]
        public Guid DutyId { get; set; }

        [Required]
        public Duty? Duty { get; set; }
    }
}
