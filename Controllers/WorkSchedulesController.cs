using dotnetstartermvc.Models;
using dotnetstartermvc.ModelsRequest.WorkScheduleRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using X.PagedList;

namespace dotnetstartermvc.Controllers
{
    [Authorize]
    [Route("/WorkSchedules/[action]")]
    public class WorkSchedulesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public WorkSchedulesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<AppUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        [TempData]
        public string StatusMessage { set; get; }

        public async Task<IActionResult> Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 10; // Số lượng item trên mỗi trang

            var workSchedule = from s in _context.WorkSchedules.Include(s => s.User)
                               select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                workSchedule = workSchedule.Where(s => s.Title.Contains(searchString) 
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Address.Contains(searchString)
                || s.Participants.Contains(searchString)
                || s.User.Firstname.Contains(searchString));
            }

            workSchedule = workSchedule.OrderByDescending(s => s.CreatedDate);

            var pagedList = await workSchedule.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            return View(pagedList);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.WorkSchedules == null)
            {
                return NotFound();
            }

            var workSchedule = await _context.WorkSchedules
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workSchedule == null)
            {
                return NotFound();
            }

            return View(workSchedule);
        }

        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                ViewData["Username"] = user.Firstname;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateWorkScheduleRequest request)
        {
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                var workSchedule = new WorkSchedule
                {
                    Title = request.Title,
                    Description = request.Description,
                    Note = request.Note,
                    Address = request.Address,
                    Participants = request.Participants,
                    ActionDate = request.ActionDate,
                    CreatedDate = DateTime.Now,
                    UserId = user.Id,
                };
                _context.Add(workSchedule);
                await _context.SaveChangesAsync();

                StatusMessage = "Tạo lịch thành công!";

                return RedirectToAction(nameof(Index));
            }

            if (user != null)
            {
                ViewData["Username"] = user.Firstname;
            }

            return View(request);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.WorkSchedules == null)
            {
                return NotFound();
            }

            var workSchedule = await _context.WorkSchedules.FindAsync(id);
            if (workSchedule == null)
            {
                return NotFound();
            }

            return View(workSchedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditWorkScheduleRequest request)
        {
            var workSchedule = await _context.WorkSchedules.FindAsync(id);
            if (workSchedule == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                workSchedule.Title = request.Title;
                workSchedule.Description = request.Description;
                workSchedule.Note = request.Note;
                workSchedule.Address = request.Address;
                workSchedule.Participants = request.Participants;
                workSchedule.ActionDate = request.ActionDate;
                workSchedule.CreatedDate = DateTime.Now;
                workSchedule.UserId = user.Id;
                
                _context.Update(workSchedule);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật lịch thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.WorkSchedules == null)
            {
                return NotFound();
            }

            var workSchedule = await _context.WorkSchedules
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workSchedule == null)
            {
                return NotFound();
            }

            return View(workSchedule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.WorkSchedules == null)
            {
                return Problem("Entity set 'DataContext.WorkSchedules'  is null.");
            }
            var workSchedule = await _context.WorkSchedules.FindAsync(id);
            if (workSchedule != null)
            {
                _context.WorkSchedules.Remove(workSchedule);
            }

            await _context.SaveChangesAsync();

            StatusMessage = "Xóa lịch thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool WorkScheduleExists(Guid id)
        {
            return (_context.WorkSchedules?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> ListSchedule(string searchString)
        {
            var workSchedules = _context.WorkSchedules.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                workSchedules = workSchedules.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Address.Contains(searchString)
                || s.Participants.Contains(searchString)
                || s.User.Firstname.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;

            return View(workSchedules);
        }

        public async Task<IActionResult> FilterByDay(string searchString)
        {
            var currentDate = DateTime.Today;
            var startOfDay = currentDate.Date;
            var endOfDay = startOfDay.AddDays(1);

            ViewBag.CurrentDate = currentDate;
            ViewBag.CurrentFilter = searchString;

            var workSchedules = _context.WorkSchedules.Where(s => s.CreatedDate >= startOfDay && s.CreatedDate < endOfDay);

            if (!string.IsNullOrEmpty(searchString))
            {
                workSchedules = workSchedules.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Address.Contains(searchString)
                || s.Participants.Contains(searchString)
                || s.User.Firstname.Contains(searchString));
            }

            return View(workSchedules);
        }

        public async Task<IActionResult> FilterByWeek(string searchString)
        {

            var currentDate = DateTime.Now.Date; // Lấy ngày hiện tại
                                                 //var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday); // Tính ngày đầu tiên của tuần là thứ Hai
                                                 //var endOfWeek = startOfWeek.AddDays(6); // Tính ngày cuối của tuần là Chủ Nhật
            var startOfWeek = currentDate.AddDays(-(7 + (int)currentDate.DayOfWeek - (int)DayOfWeek.Monday) % 7);
            var endOfWeek = startOfWeek.AddDays(6);

            // Tính toán số tuần
            var weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = endOfWeek;
            ViewBag.WeekNumber = weekNumber;
            ViewBag.CurrentFilter = searchString;

            var workSchedules = _context.WorkSchedules.Where(s => s.CreatedDate >= startOfWeek && s.CreatedDate <= endOfWeek.AddDays(1));

            if (!string.IsNullOrEmpty(searchString))
            {
                workSchedules = workSchedules.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Address.Contains(searchString)
                || s.Participants.Contains(searchString)
                || s.User.Firstname.Contains(searchString));
            }

            return View(workSchedules);
        }
    }
}
