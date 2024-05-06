using dotnetstartermvc.Data;
using dotnetstartermvc.Models;
using dotnetstartermvc.ModelsRequest.AssignmentRequest;
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
    [Route("/Assignments/[action]")]
    public class AssignmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AssignmentsController(AppDbContext context, UserManager<AppUser> userManager)
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

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 10; // Số lượng item trên mỗi trang

            var assignments = from s in _context.Assignments.Include(s => s.User)
                              select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                assignments = assignments.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Feedback.Contains(searchString)
                || s.User.Email.Contains(searchString));
            }

            assignments = assignments.OrderByDescending(s => s.ActionDate);

            var pagedList = await assignments.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            return View(pagedList);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                ViewData["Username"] = user.Email;
            }
            return View();
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAssignmentRequest request)
        {
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                var assignment = new Assignment
                {
                    Title = request.Title,
                    Description = request.Description,
                    Note = request.Note,
                    ActionDate = request.ActionDate,
                    IsComplete = false,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UserId = user.Id,
                };
                _context.Add(assignment);
                await _context.SaveChangesAsync();

                StatusMessage = "Tạo nhiệm vụ mới thành công!";

                return RedirectToAction(nameof(Index));
            }

            if (user != null)
            {
                ViewData["Username"] = user.Email;
            }

            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var assignmentEdit = new EditAssignmentRequest
            {
                Id = assignment.Id,
                Title = assignment.Title,
                Description = assignment.Description,
                Note = assignment.Note,
                ActionDate = assignment.ActionDate,
                IsComplete = assignment.IsComplete,
            };

            return View(assignmentEdit);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditAssignmentRequest request)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                assignment.Title = request.Title;
                assignment.Description = request.Description;
                assignment.Note = request.Note;
                assignment.ActionDate = request.ActionDate;
                assignment.IsComplete = request.IsComplete;
                assignment.UpdatedDate = DateTime.Now;
                assignment.UserId = user.Id;

                _context.Update(assignment);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật nhiệm vụ thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Assignments == null)
            {
                return Problem("Entity set 'DataContext.Assignments'  is null.");
            }
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
            }

            await _context.SaveChangesAsync();

            StatusMessage = "Xóa nhiệm vụ thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(Guid id)
        {
            return (_context.Assignments?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> ListAssignments(int? page, string searchString)
        {
            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 10; // Số lượng item trên mỗi trang

            var assignments = from s in _context.Assignments.Include(s => s.User)
                              select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                assignments = assignments.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Feedback.Contains(searchString)
                || s.User.Email.Contains(searchString));
            }

            assignments = assignments.OrderByDescending(s => s.ActionDate);

            var pagedList = await assignments.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            return View(pagedList);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> FilterByDay(string searchString)
        {
            var currentDate = DateTime.Today;
            var startOfDay = currentDate.Date;
            var endOfDay = startOfDay.AddDays(1);

            ViewBag.CurrentDate = currentDate;
            ViewBag.CurrentFilter = searchString;

            var assignments = _context.Assignments.Include(s => s.User).Where(s => s.ActionDate >= startOfDay && s.ActionDate < endOfDay);

            if (!string.IsNullOrEmpty(searchString))
            {
                assignments = assignments.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Feedback.Contains(searchString)
                || s.User.Email.Contains(searchString));
            }

            assignments = assignments.OrderBy(s => s.ActionDate);

            return View(assignments);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> FilterByWeek(string searchString)
        {

            var currentDate = DateTime.Now.Date;
            var startOfWeek = currentDate.AddDays(-(7 + (int)currentDate.DayOfWeek - (int)DayOfWeek.Monday) % 7);
            var endOfWeek = startOfWeek.AddDays(6);

            var weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = endOfWeek;
            ViewBag.WeekNumber = weekNumber;
            ViewBag.CurrentFilter = searchString;

            var assignments = _context.Assignments.Include(s => s.User).Where(s => s.ActionDate >= startOfWeek && s.ActionDate <= endOfWeek.AddDays(1));

            if (!string.IsNullOrEmpty(searchString))
            {
                assignments = assignments.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Feedback.Contains(searchString)
                || s.User.Email.Contains(searchString));
            }

            assignments = assignments.OrderBy(s => s.ActionDate);

            return View(assignments);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> EditIsComplete(Guid? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditIsComplete(Guid id, EditIsCompleteAssignmentRequest request)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                assignment.IsComplete = request.IsComplete;
                assignment.Feedback = request.Feedback;

                _context.Update(assignment);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật tiến độ nhiệm vụ thành công!";

                return RedirectToAction(nameof(ListAssignments));
            }

            return View(request);
        }
    }
}
