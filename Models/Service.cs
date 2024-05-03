using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnetstartermvc.Models
{
    public class Service : BaseEntity
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime? CreatedDate { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [Required]
        public AppUser? User { get; set; }

        public List<ServicePhoto>? ServicePhotos { get; set; }
    }
}
