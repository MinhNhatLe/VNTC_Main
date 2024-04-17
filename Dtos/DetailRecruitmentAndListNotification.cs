using dotnetstartermvc.Models;

namespace dotnetstartermvc.Dtos
{
    public class DetailRecruitmentAndListNotification
    {
        public Recruitment DetailRecruitment { get; set; }
        public List<Notification> ListNotifications { get; set; }
    }
}
