using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.Models
{
    public class UserCustomer
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public AppUser? User { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Customer? Customer { get; set; }
    }
}
