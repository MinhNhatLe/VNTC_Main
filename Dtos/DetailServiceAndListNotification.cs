using dotnetstartermvc.Models;

namespace dotnetstartermvc.Dtos
{
    public class DetailServiceAndListNotification
    {
        public Service DetailService { get; set; }
        public List<Notification> ListNotifications { get; set; }
    }
}
