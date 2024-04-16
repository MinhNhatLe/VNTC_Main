using dotnetstartermvc.Models;

namespace dotnetstartermvc.Dtos
{
    public class DetailNotificationAndListNotification
    {
        public Notification DetailNotifications { get; set; }
        public List<Notification> ListNotifications { get; set; }
    }
}
