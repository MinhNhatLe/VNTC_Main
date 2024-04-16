using dotnetstartermvc.Data;
using dotnetstartermvc.Models;
using dotnetstartermvc.ModelsRequest.RecruitmentRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using X.PagedList;

namespace dotnetstartermvc.Controllers
{
    [Authorize]
    [Route("/Recruitments/[action]")]
    public class RecruitmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RecruitmentsController(AppDbContext context, UserManager<AppUser> userManager)
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

            var recruitments = from s in _context.Recruitments.Include(s => s.User)
                               select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                recruitments = recruitments.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString) || s.User.Email.Contains(searchString));
            }

            recruitments = recruitments.OrderByDescending(s => s.CreatedDate);

            var pagedList = await recruitments.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            return View(pagedList);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Details(Guid? id)
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

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
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
        public async Task<IActionResult> Create(CreateRecruitmentRequest request)
        {
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                var recruitment = new Recruitment
                {
                    Title = request.Title,
                    Quantity = request.Quantity,
                    Description = request.Description,
                    CreatedDate = request.CreatedDate,
                    UserId = user.Id,

                };
                _context.Add(recruitment);
                await _context.SaveChangesAsync();

                StatusMessage = "Tạo tin tuyển dụng thành công!";

                return RedirectToAction(nameof(Index));
            }

            if (user != null)
            {
                ViewData["Username"] = user.Email;
            }

            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Recruitments == null)
            {
                return NotFound();
            }

            var recruitment = await _context.Recruitments.FindAsync(id);
            if (recruitment == null)
            {
                return NotFound();
            }

            return View(recruitment);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditRecruitmentRequest request)
        {
            var recruitment = await _context.Recruitments.FindAsync(id);
            if (recruitment == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                recruitment.Title = request.Title;
                recruitment.Quantity = request.Quantity;
                recruitment.Description = request.Description;
                recruitment.CreatedDate = request.CreatedDate;
                recruitment.UserId = user.Id;

                _context.Update(recruitment);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật tin tuyển dụng thành công!";

                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Delete(Guid? id)
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

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        // POST: Recruitments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Recruitments == null)
            {
                return Problem("Entity set 'DataContext.Recruitments'  is null.");
            }
            var recruitment = await _context.Recruitments.FindAsync(id);
            if (recruitment != null)
            {
                _context.Recruitments.Remove(recruitment);
            }

            await _context.SaveChangesAsync();

            StatusMessage = "Xóa tin tuyển dụng thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool RecruitmentExists(Guid id)
        {
            return (_context.Recruitments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
