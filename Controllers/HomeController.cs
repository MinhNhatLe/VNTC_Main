using dotnetstartermvc.Dtos;
using dotnetstartermvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace dotnetstartermvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.Include(s => s.User).OrderByDescending(s => s.CreatedDate).Take(3).ToListAsync();

            return View(services);
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Service(int? page, string searchString)
        {
            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 6; // Số lượng item trên mỗi trang

            var services = from s in _context.Services.Include(s => s.User)
                           select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                services = services.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString) || s.User.Email.Contains(searchString));
            }

            var pagedServices = await services.OrderByDescending(s => s.CreatedDate).ToPagedListAsync(pageNumber, pageSize);
            ViewData["CurrentFilter"] = searchString;

            return View(pagedServices);
        }


        public async Task<IActionResult> ServiceDetails(Guid? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            var notifications = await _context.Notifications
                                    .OrderByDescending(n => n.CreatedDate)
                                    .Take(10)
                                    .ToListAsync();

            var viewModel = new DetailServiceAndListNotification
            {
                DetailService = service,
                ListNotifications = notifications
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Notifications(int? page, string searchString)
        {
            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 6; // Số lượng item trên mỗi trang

            var notifications = from n in _context.Notifications.Include(s => s.User)
                       select n;

            if (!string.IsNullOrEmpty(searchString))
            {
                notifications = notifications.Where(n => n.Title.Contains(searchString) || n.Description.Contains(searchString) || n.User.Email.Contains(searchString));
            }

            var pagedNotifications = await notifications.OrderByDescending(s => s.CreatedDate).ToPagedListAsync(pageNumber, pageSize);

            ViewData["CurrentFilter"] = searchString;

            return View(pagedNotifications);
        }


        public async Task<IActionResult> NotificationDetails(Guid? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            var notifications = await _context.Notifications
                                    .OrderByDescending(n => n.CreatedDate)
                                    .Take(10)
                                    .ToListAsync();

            var viewModel = new DetailNotificationAndListNotification
            {
                DetailNotifications = notification,
                ListNotifications = notifications
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Recruitments(int? page, string searchString)
        {
            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 6; // Số lượng item trên mỗi trang

            var recruitments = from n in _context.Recruitments.Include(s => s.User)
                               select n;

            if (!string.IsNullOrEmpty(searchString))
            {
                recruitments = recruitments.Where(n => n.Title.Contains(searchString) || n.Description.Contains(searchString) || n.User.Email.Contains(searchString));
            }

            var pagedRecruitments = await recruitments.OrderByDescending(s => s.CreatedDate).ToPagedListAsync(pageNumber, pageSize);

            ViewData["CurrentFilter"] = searchString;

            return View(pagedRecruitments);
        }


        public async Task<IActionResult> RecruitmentDetails(Guid? id)
        {
            if (id == null || _context.Recruitments == null)
            {
                return NotFound();
            }

            var recruitment = await _context.Recruitments
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recruitment == null)
            {
                return NotFound();
            }

            return View(recruitment);
        }

        public IActionResult WhyUs()
        {
            return View();
        }
        public IActionResult Team()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}