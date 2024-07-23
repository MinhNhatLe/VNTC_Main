using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnetstartermvc.Models
{
    public class BidPackage : BaseEntity
    {
        [Required]
        public string? Name { get; set; }

        public string? Notes { get; set; }

        [Required]
        public DateTime? CreatedDate { get; set; }

        [Required]
        public DateTime? UpdatedDate { get; set; }

        [Required]
        public string UserId { get; set; }

        public Guid CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
