using dotnetstartermvc.Data;
using dotnetstartermvc.Models;
using dotnetstartermvc.ModelsRequest.NotificationRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using X.PagedList;

namespace dotnetstartermvc.Controllers
{
    [Authorize]
    [Route("/Notifications/[action]")]
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public NotificationsController(AppDbContext context, UserManager<AppUser> userManager)
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

            var notifications = from n in _context.Notifications.Include(s => s.User)
                                select n;

            if (!string.IsNullOrEmpty(searchString))
            {
                notifications = notifications.Where(n => n.Title.Contains(searchString) || n.Description.Contains(searchString) || n.User.Email.Contains(searchString));
            }

            notifications = notifications.OrderByDescending(s => s.CreatedDate);

            var pagedList = await notifications.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            return View(pagedList);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var @new = await _context.Notifications
                .Include(s => s.User)
                .Include(s => s.NotificationPhotos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@new == null)
            {
                return NotFound();
            }

            return View(@new);
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
        public async Task<IActionResult> Create(CreateNotificationRequest request)
        {
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                var notifications = new Notification
                {
                    Title = request.Title,
                    Description = request.Description,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UserId = user.Id,
                };
                _context.Add(notifications);
                await _context.SaveChangesAsync();

                StatusMessage = "Tạo thông báo thành công!";

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
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notifications = await _context.Notifications.FindAsync(id);
            if (notifications == null)
            {
                return NotFound();
            }

            var notificationEdit = new EditNotificationRequest
            {
                Id = notifications.Id,
                Title = notifications.Title,
                Description = notifications.Description,
            };

            return View(notificationEdit);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditNotificationRequest request)
        {
            var notifications = await _context.Notifications.FindAsync(id);
            if (notifications == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                notifications.Title = request.Title;
                notifications.Description = request.Description;
                notifications.UpdatedDate = DateTime.Now;
                notifications.UserId = user.Id;

                _context.Update(notifications);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật thông báo thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(s => s.User)
                .Include(s => s.NotificationPhotos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Notifications == null)
            {
                return Problem("Entity set 'DataContext.Notifications'  is null.");
            }
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();

            StatusMessage = "Xóa thông báo thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(Guid id)
        {
            return (_context.Notifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet]
        public IActionResult UploadPhoto(Guid id)
        {
            var notification = _context.Notifications.Where(e => e.Id == id)
                            .Include(p => p.NotificationPhotos)
                            .FirstOrDefault();

            if (notification == null)
            {
                return NotFound("Không có thông báo");
            }

            ViewData["notification"] = notification;
            return View(new UploadOneFile());
        }

        [HttpPost, ActionName("UploadPhoto")]
        public async Task<IActionResult> UploadPhotoAsync(Guid id, [Bind("FileUpload")] UploadOneFile f)
        {
            var notification = _context.Notifications.Where(e => e.Id == id)
                .Include(p => p.NotificationPhotos)
                .FirstOrDefault();

            if (notification == null)
            {
                return NotFound("Không có thông báo");
            }

            ViewData["notification"] = notification;

            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine("Uploads", "Notifications", file1);

                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }

                _context.Add(new NotificationPhoto()
                {
                    NotificationId = notification.Id,
                    FileName = file1
                });

                await _context.SaveChangesAsync();
            }

            return View(f);
        }

        [HttpPost]
        public IActionResult ListPhotos(Guid id)
        {
            var notification = _context.Notifications.Where(e => e.Id == id)
                .Include(p => p.NotificationPhotos)
                .FirstOrDefault();

            if (notification == null)
            {
                return Json(
                    new
                    {
                        success = 0,
                        message = "Notification not found",
                    }
                );
            }

            var listphotos = notification.NotificationPhotos.Select(photo => new
            {
                id = photo.Id,
                path = "/contents/Notifications/" + photo.FileName
            });

            return Json(
                new
                {
                    success = 1,
                    photos = listphotos
                }
            );
        }

        [HttpPost]
        public IActionResult DeletePhoto(Guid id)
        {
            var photo = _context.NotificationPhotos.Where(p => p.Id == id).FirstOrDefault();
            if (photo != null)
            {
                _context.Remove(photo);
                _context.SaveChanges();

                var filename = "Uploads/Notifications/" + photo.FileName;
                System.IO.File.Delete(filename);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhotoApi(Guid id, [Bind("FileUpload")] UploadOneFile f)
        {
            var notification = _context.Notifications.Where(e => e.Id == id)
                .Include(p => p.NotificationPhotos)
                .FirstOrDefault();

            if (notification == null)
            {
                return NotFound("Không có thông báo");
            }

            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine("Uploads", "Notifications", file1);

                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }

                _context.Add(new NotificationPhoto()
                {
                    NotificationId = notification.Id,
                    FileName = file1
                });

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
