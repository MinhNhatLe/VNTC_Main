using dotnetstartermvc.Models;

namespace dotnetstartermvc.ModelsRequest.CustomerRequest
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; }
        public List<UserCustomer> UserCustomers { get; set; }
    }
}
