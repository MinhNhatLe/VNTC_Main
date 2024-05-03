using System.ComponentModel.DataAnnotations.Schema;

namespace dotnetstartermvc.Models
{
    public class NotificationPhoto : BaseEntity
    {
        // abc.png, 123.jpg ... 
        // => /contents/Products/abc.pgn, /contents/Products/123.jpg
        public string FileName { get; set; }

        public Guid NotificationId { get; set; }

        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; }
    }
}
