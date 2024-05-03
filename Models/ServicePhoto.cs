using System.ComponentModel.DataAnnotations.Schema;

namespace dotnetstartermvc.Models
{
    public class ServicePhoto : BaseEntity
    {
        // abc.png, 123.jpg ... 
        // => /contents/Products/abc.pgn, /contents/Products/123.jpg
        public string FileName { get; set; }

        public Guid ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }
    }
}
