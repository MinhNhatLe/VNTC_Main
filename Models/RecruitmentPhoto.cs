using System.ComponentModel.DataAnnotations.Schema;

namespace dotnetstartermvc.Models
{
    public class RecruitmentPhoto : BaseEntity
    {
        // abc.png, 123.jpg ... 
        // => /contents/Products/abc.pgn, /contents/Products/123.jpg
        public string FileName { get; set; }

        public Guid RecruitmentId { get; set; }

        [ForeignKey("RecruitmentId")]
        public Recruitment Recruitment { get; set; }
    }
}
