using dotnetstartermvc.Data;
using dotnetstartermvc.Models;
using dotnetstartermvc.ModelsRequest.DutyRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using X.PagedList;

namespace dotnetstartermvc.Controllers
{
    [Authorize]
    [Route("/Duties/[action]")]
    public class DutiesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public DutiesController(AppDbContext context, UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
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

            var duties = from s in _context.Duties
                         select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                duties = duties.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Feedback.Contains(searchString));
            }

            duties = duties.OrderByDescending(s => s.ActionDate);

            var pagedList = await duties.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            var dutyViewModels = pagedList.Select(duty => new DutyViewModel
            {
                Duty = duty,
                UserDuties = _context.UserDuties.Include(s => s.User).Where(ud => ud.DutyId == duty.Id).ToList()
            });

            return View(dutyViewModels);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Duties == null)
            {
                return NotFound();
            }

            var duty = await _context.Duties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (duty == null)
            {
                return NotFound();
            }

            var dutyViewModel = new DutyViewModel
            {
                Duty = duty,
                UserDuties = _context.UserDuties.Include(s => s.User).Where(ud => ud.DutyId == duty.Id).ToList()
            };

            return View(dutyViewModel);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            List<string> emails = await _context.Users.Select(r => r.Email).ToListAsync();
            ViewBag.allEmails = new SelectList(emails);

            return View();
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDutyRequest request)
        {
            if (ModelState.IsValid)
            {
                var duty = new Duty
                {
                    Title = request.Title,
                    Description = request.Description,
                    Note = request.Note,
                    ActionDate = request.ActionDate,
                    IsComplete = false,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                };
                _context.Add(duty);
                await _context.SaveChangesAsync();

                if (request.Emails != null)
                {
                    foreach (var email in request.Emails)
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            var userDuty = new UserDuty
                            {
                                UserId = user.Id,
                                DutyId = duty.Id
                            };
                            _context.UserDuties.Add(userDuty);

                            string subject = "Có 1 nhiệm vụ mới được giao cho bạn từ VNTC";
                            string message = "Vui lòng đăng nhập vào hệ thống và kiểm tra thông tin nhiệm vụ vừa mới được giao từ VNTC! Xin cảm ơn.";
                            await _emailSender.SendEmailAsync(email, subject, message);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                StatusMessage = "Tạo nhiệm vụ riêng mới thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Duties == null)
            {
                return NotFound();
            }

            var duty = await _context.Duties.FindAsync(id);
            if (duty == null)
            {
                return NotFound();
            }

            List<string> emails = await _context.Users.Select(r => r.Email).ToListAsync();
            ViewBag.allEmails = new SelectList(emails);

            var assignedEmails = await _context.UserDuties
                .Where(ud => ud.DutyId == id)
                .Select(ud => ud.User.Email)
                .ToListAsync();

            var dutyEdit = new EditDutyRequest
            {
                Id = duty.Id,
                Title = duty.Title,
                Description = duty.Description,
                Note = duty.Note,
                ActionDate = duty.ActionDate,
                IsComplete = duty.IsComplete,
                Emails = assignedEmails.ToArray(),
            };

            return View(dutyEdit);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditDutyRequest request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            var duty = await _context.Duties.FindAsync(id);
            if (duty == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                duty.Title = request.Title;
                duty.Description = request.Description;
                duty.Note = request.Note;
                duty.ActionDate = request.ActionDate;
                duty.IsComplete = request.IsComplete;
                duty.UpdatedDate = DateTime.Now;

                _context.Update(duty);
                await _context.SaveChangesAsync();

                _context.UserDuties.RemoveRange(_context.UserDuties.Where(ud => ud.DutyId == id));

                if (request.Emails != null)
                {
                    foreach (var email in request.Emails)
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            var userDuty = new UserDuty
                            {
                                UserId = user.Id,
                                DutyId = duty.Id
                            };
                            _context.UserDuties.Add(userDuty);

                            string subject = "Có 1 nhiệm vụ mới được giao cho bạn từ VNTC";
                            string message = "Vui lòng đăng nhập vào hệ thống và kiểm tra thông tin nhiệm vụ vừa mới được giao từ VNTC! Xin cảm ơn.";
                            await _emailSender.SendEmailAsync(email, subject, message);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật nhiệm vụ thành công!";

                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Duties == null)
            {
                return NotFound();
            }

            var duty = await _context.Duties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (duty == null)
            {
                return NotFound();
            }

            var dutyViewModel = new DutyViewModel
            {
                Duty = duty,
                UserDuties = _context.UserDuties.Include(s => s.User).Where(ud => ud.DutyId == duty.Id).ToList()
            };

            return View(dutyViewModel);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Duties == null)
            {
                return Problem("Entity set 'DataContext.Duties'  is null.");
            }
            var duty = await _context.Duties.FindAsync(id);
            if (duty != null)
            {
                _context.Duties.Remove(duty);
            }

            await _context.SaveChangesAsync();

            StatusMessage = "Xóa nhiệm vụ thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(Guid id)
        {
            return (_context.Duties?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> ListDuties(int? page, string searchString)
        {
            var user = await GetCurrentUserAsync();

            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 10; // Số lượng item trên mỗi trang

            var duties = from s in _context.Duties
                         join ud in _context.UserDuties on s.Id equals ud.DutyId
                         where ud.UserId == user.Id
                         select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                duties = duties.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Feedback.Contains(searchString));
            }

            duties = duties.OrderByDescending(s => s.ActionDate);

            var pagedList = await duties.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            var dutyViewModels = pagedList.Select(duty => new DutyViewModel
            {
                Duty = duty,
                UserDuties = _context.UserDuties.Include(s => s.User).Where(ud => ud.DutyId == duty.Id).ToList()
            });

            return View(dutyViewModels);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> FilterByDay(string searchString)
        {
            var user = await GetCurrentUserAsync();

            var currentDate = DateTime.Today;
            var startOfDay = currentDate.Date;
            var endOfDay = startOfDay.AddDays(1);

            ViewBag.CurrentDate = currentDate;
            ViewBag.CurrentFilter = searchString;

            var duties = from s in _context.Duties
                         join ud in _context.UserDuties on s.Id equals ud.DutyId
                         where ud.UserId == user.Id && s.ActionDate >= startOfDay && s.ActionDate < endOfDay
                         select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                duties = duties.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Feedback.Contains(searchString));
            }

            duties = duties.OrderBy(s => s.ActionDate);

            var dutyViewModels = duties.Select(duty => new DutyViewModel
            {
                Duty = duty,
                UserDuties = _context.UserDuties.Include(s => s.User).Where(ud => ud.DutyId == duty.Id).ToList()
            });

            return View(dutyViewModels);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> FilterByWeek(string searchString)
        {
            var user = await GetCurrentUserAsync();

            var currentDate = DateTime.Now.Date;
            var startOfWeek = currentDate.AddDays(-(7 + (int)currentDate.DayOfWeek - (int)DayOfWeek.Monday) % 7);
            var endOfWeek = startOfWeek.AddDays(6);

            var weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = endOfWeek;
            ViewBag.WeekNumber = weekNumber;
            ViewBag.CurrentFilter = searchString;


            var duties = from s in _context.Duties
                         join ud in _context.UserDuties on s.Id equals ud.DutyId
                         where ud.UserId == user.Id && s.ActionDate >= startOfWeek && s.ActionDate <= endOfWeek.AddDays(1)
                         select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                duties = duties.Where(s => s.Title.Contains(searchString)
                || s.Description.Contains(searchString)
                || s.Note.Contains(searchString)
                || s.Feedback.Contains(searchString));
            }

            duties = duties.OrderBy(s => s.ActionDate);

            var dutyViewModels = duties.Select(duty => new DutyViewModel
            {
                Duty = duty,
                UserDuties = _context.UserDuties.Include(s => s.User).Where(ud => ud.DutyId == duty.Id).ToList()
            });

            return View(dutyViewModels);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> EditIsComplete(Guid? id)
        {
            var user = await GetCurrentUserAsync();

            if (id == null || _context.Duties == null)
            {
                return NotFound();
            }

            var duty = await _context.Duties.FindAsync(id);
            if (duty == null)
            {
                return NotFound();
            }

            if (user == null || !_context.UserDuties.Any(ud => ud.UserId == user.Id && ud.DutyId == id))
            {
                return Forbid();
            }

            return View(duty);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditIsComplete(Guid id, EditIsCompleteDutyRequest request)
        {
            var user = await GetCurrentUserAsync();

            var duty = await _context.Duties.FindAsync(id);
            if (duty == null)
            {
                return NotFound();
            }

            if (user == null || !_context.UserDuties.Any(ud => ud.UserId == user.Id && ud.DutyId == id))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                duty.IsComplete = request.IsComplete;
                duty.Feedback = request.Feedback;

                _context.Update(duty);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật tiến độ nhiệm vụ thành công!";

                return RedirectToAction(nameof(ListDuties));
            }

            return View(request);
        }
    }
}
