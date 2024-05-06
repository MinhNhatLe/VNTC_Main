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
                .Include(s => s.RecruitmentPhotos)
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
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
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

            var recruitmentEdit = new EditRecruitmentRequest
            {
                Id = recruitment.Id,
                Title = recruitment.Title,
                Quantity = recruitment.Quantity,
                Description = recruitment.Description,
            };

            return View(recruitmentEdit);
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
                recruitment.UpdatedDate = DateTime.Now;
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
                .Include(s => s.RecruitmentPhotos)
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

        [HttpGet]
        public IActionResult UploadPhoto(Guid id)
        {
            var recruitment = _context.Recruitments.Where(e => e.Id == id)
                            .Include(p => p.RecruitmentPhotos)
                            .FirstOrDefault();

            if (recruitment == null)
            {
                return NotFound("Không có tuyển dụng");
            }

            ViewData["recruitment"] = recruitment;
            return View(new UploadOneFile());
        }

        [HttpPost, ActionName("UploadPhoto")]
        public async Task<IActionResult> UploadPhotoAsync(Guid id, [Bind("FileUpload")] UploadOneFile f)
        {
            var recruitment = _context.Recruitments.Where(e => e.Id == id)
                .Include(p => p.RecruitmentPhotos)
                .FirstOrDefault();

            if (recruitment == null)
            {
                return NotFound("Không có tuyển dụng");
            }

            ViewData["recruitment"] = recruitment;

            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine("Uploads", "Recruitments", file1);

                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }

                _context.Add(new RecruitmentPhoto()
                {
                    RecruitmentId = recruitment.Id,
                    FileName = file1
                });

                await _context.SaveChangesAsync();
            }

            return View(f);
        }

        [HttpPost]
        public IActionResult ListPhotos(Guid id)
        {
            var recruitment = _context.Recruitments.Where(e => e.Id == id)
                .Include(p => p.RecruitmentPhotos)
                .FirstOrDefault();

            if (recruitment == null)
            {
                return Json(
                    new
                    {
                        success = 0,
                        message = "Recruitment not found",
                    }
                );
            }

            var listphotos = recruitment.RecruitmentPhotos.Select(photo => new
            {
                id = photo.Id,
                path = "/contents/Recruitments/" + photo.FileName
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
            var photo = _context.RecruitmentPhotos.Where(p => p.Id == id).FirstOrDefault();
            if (photo != null)
            {
                _context.Remove(photo);
                _context.SaveChanges();

                var filename = "Uploads/Recruitments/" + photo.FileName;
                System.IO.File.Delete(filename);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhotoApi(Guid id, [Bind("FileUpload")] UploadOneFile f)
        {
            var recruitment = _context.Recruitments.Where(e => e.Id == id)
                .Include(p => p.RecruitmentPhotos)
                .FirstOrDefault();

            if (recruitment == null)
            {
                return NotFound("Không có tuyển dụng");
            }

            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine("Uploads", "Recruitments", file1);

                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }

                _context.Add(new RecruitmentPhoto()
                {
                    RecruitmentId = recruitment.Id,
                    FileName = file1
                });

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
