using dotnetstartermvc.Models;

namespace dotnetstartermvc.ModelsRequest.DutyRequest
{
    public class DutyViewModel
    {
        public Guid Id { get; set; }
        public Duty Duty { get; set; }
        public List<UserDuty> UserDuties { get; set; }
    }
}
