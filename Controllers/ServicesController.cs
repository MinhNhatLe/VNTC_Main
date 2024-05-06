using dotnetstartermvc.Data;
using dotnetstartermvc.Models;
using dotnetstartermvc.ModelsRequest.ServicesRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using X.PagedList;

namespace dotnetstartermvc.Controllers
{
    [Authorize]
    [Route("/Services/[action]")]
    public class ServicesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ServicesController(AppDbContext context, UserManager<AppUser> userManager)
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

            var services = from s in _context.Services.Include(s => s.User)
                           select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                services = services.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString) || s.User.Email.Contains(searchString));
            }

            services = services.OrderByDescending(s => s.CreatedDate);

            var pagedList = await services.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            return View(pagedList);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.User)
                .Include(s => s.ServicePhotos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
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
        public async Task<IActionResult> Create(CreateServicesRequest request)
        {
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    Title = request.Title,
                    Description = request.Description,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UserId = user.Id,
                };
                _context.Add(service);
                await _context.SaveChangesAsync();

                StatusMessage = "Tạo dịch vụ mới thành công!";

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
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var serviceEdit = new EditServicesRequest
            {
                Id = service.Id,
                Title = service.Title,
                Description = service.Description,
            };

            return View(serviceEdit);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditServicesRequest request)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                service.Title = request.Title;
                service.Description = request.Description;
                service.UpdatedDate = DateTime.Now;
                service.UserId = user.Id;

                _context.Update(service);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật dịch vụ thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.User)
                .Include(s => s.ServicePhotos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Services == null)
            {
                return Problem("Entity set 'DataContext.Services'  is null.");
            }
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
            }

            await _context.SaveChangesAsync();

            StatusMessage = "Xóa dịch vụ thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(Guid id)
        {
            return (_context.Services?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet]
        public IActionResult UploadPhoto(Guid id)
        {
            var service = _context.Services.Where(e => e.Id == id)
                            .Include(p => p.ServicePhotos)
                            .FirstOrDefault();

            if (service == null)
            {
                return NotFound("Không có dịch vụ");
            }

            ViewData["service"] = service;
            return View(new UploadOneFile());
        }

        [HttpPost, ActionName("UploadPhoto")]
        public async Task<IActionResult> UploadPhotoAsync(Guid id, [Bind("FileUpload")] UploadOneFile f)
        {
            var service = _context.Services.Where(e => e.Id == id)
                .Include(p => p.ServicePhotos)
                .FirstOrDefault();

            if (service == null)
            {
                return NotFound("Không có dịch vụ");
            }

            ViewData["service"] = service;

            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine("Uploads", "Services", file1);

                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }

                _context.Add(new ServicePhoto()
                {
                    ServiceId = service.Id,
                    FileName = file1
                });

                await _context.SaveChangesAsync();
            }

            return View(f);
        }

        [HttpPost]
        public IActionResult ListPhotos(Guid id)
        {
            var service = _context.Services.Where(e => e.Id == id)
                .Include(p => p.ServicePhotos)
                .FirstOrDefault();

            if (service == null)
            {
                return Json(
                    new
                    {
                        success = 0,
                        message = "Service not found",
                    }
                );
            }

            var listphotos = service.ServicePhotos.Select(photo => new
            {
                id = photo.Id,
                path = "/contents/Services/" + photo.FileName
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
            var photo = _context.ServicePhotos.Where(p => p.Id == id).FirstOrDefault();
            if (photo != null)
            {
                _context.Remove(photo);
                _context.SaveChanges();

                var filename = "Uploads/Services/" + photo.FileName;
                System.IO.File.Delete(filename);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhotoApi(Guid id, [Bind("FileUpload")] UploadOneFile f)
        {
            var service = _context.Services.Where(e => e.Id == id)
                .Include(p => p.ServicePhotos)
                .FirstOrDefault();

            if (service == null)
            {
                return NotFound("Không có dịch vụ");
            }

            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine("Uploads", "Services", file1);

                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }

                _context.Add(new ServicePhoto()
                {
                    ServiceId = service.Id,
                    FileName = file1
                });

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
