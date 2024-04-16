using dotnetstartermvc.Data;
using dotnetstartermvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnetstartermvc.Controllers
{
    [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
    [Route("/Chart/[action]")]
    public class ChartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ChartController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var rolesCount = await _context.Roles.CountAsync();
            var usersCount = await _context.Users.CountAsync();
            var servicesCount = await _context.Services.CountAsync();
            var notificationsCount = await _context.Notifications.CountAsync();
            var jobCount = await _context.Recruitments.CountAsync();
            var workScheduleCount = await _context.WorkSchedules.CountAsync();
            var assignmentCount = await _context.Assignments.CountAsync();

            ViewBag.RolesCount = rolesCount;
            ViewBag.UsersCount = usersCount;
            ViewBag.ServicesCount = servicesCount;
            ViewBag.NotificationsCount = notificationsCount;
            ViewBag.JobCount = jobCount;
            ViewBag.WorkScheduleCount = workScheduleCount;
            ViewBag.AssignmentCount = assignmentCount;

            return View();
        }
    }
}
