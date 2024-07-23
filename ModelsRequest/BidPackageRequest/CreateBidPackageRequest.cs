using System.ComponentModel.DataAnnotations;

namespace dotnetstartermvc.ModelsRequest.BidPackageRequest
{
    public class CreateBidPackageRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên gối thầu")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Vui lòng nhập tên gối thầu từ 2 đến 200 kí tự")]
        public string Name { get; set; }
        public string? Notes { get; set; }
        public Guid CustomerId { get; set; }
    }
}
