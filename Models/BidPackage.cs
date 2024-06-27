using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnetstartermvc.Models
{
    public class BidPackage : BaseEntity
    {
        [Required]
        public string? Name { get; set; }

        public Guid CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
