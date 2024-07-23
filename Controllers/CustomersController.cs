using dotnetstartermvc.Data;
using dotnetstartermvc.Models;
using dotnetstartermvc.ModelsRequest.BidPackageRequest;
using dotnetstartermvc.ModelsRequest.CustomerRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace dotnetstartermvc.Controllers
{
    [Authorize]
    [Route("/Customers/[action]")]
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public CustomersController(AppDbContext context, UserManager<AppUser> userManager, IEmailSender emailSender)
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

            var customers = from s in _context.Customers
                            select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(s => s.CompanyName.Contains(searchString)
                || s.Address.Contains(searchString)
                || s.ProvinceOrCity.Contains(searchString)
                || s.CompanyPhoneNumber.Contains(searchString)
                || s.ContactPersonName.Contains(searchString)
                || s.Position.Contains(searchString)
                || s.ContactPersonPhoneNumber.Contains(searchString)
                || s.Email.Contains(searchString)
                || s.BidPackageName.Contains(searchString)
                || s.ProjectValue.Contains(searchString)
                || s.OpportunitySource.Contains(searchString)
                || s.Notes.Contains(searchString)
                || s.Feedback.Contains(searchString));
            }

            customers = customers.OrderByDescending(s => s.CreatedDate);

            var pagedList = await customers.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            var customerViewModels = pagedList.Select(customer => new CustomerViewModel
            {
                Customer = customer,
                UserCustomers = _context.UserCustomers.Include(s => s.User).Where(ud => ud.CustomerId == customer.Id).ToList()
            });

            return View(customerViewModels);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.BidPackages)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            var customerViewModel = new CustomerViewModel
            {
                Customer = customer,
                UserCustomers = _context.UserCustomers.Include(s => s.User).Where(ud => ud.CustomerId == customer.Id).ToList()
            };

            return View(customerViewModel);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<string> emails = await _context.Users.Select(r => r.Email).ToListAsync();
            ViewBag.allEmails = new SelectList(emails);

            var currentUser = await GetCurrentUserAsync();
            if (currentUser != null)
            {
                ViewData["Username"] = currentUser.Email;
            }

            return View();
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerRequest request)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                var customer = new Customer
                {
                    CompanyName = request.CompanyName,
                    Address = request.Address,
                    ProvinceOrCity = request.ProvinceOrCity,
                    CompanyPhoneNumber = request.CompanyPhoneNumber,
                    ContactPersonName = request.ContactPersonName,
                    Position = request.Position,
                    ContactPersonPhoneNumber = request.ContactPersonPhoneNumber,
                    Email = request.Email,
                    BidPackageName = request.BidPackageName,
                    ProjectValue = request.ProjectValue,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    OpportunitySource = request.OpportunitySource,
                    Notes = request.Notes,
                    IsComplete = false,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UserId = currentUser.Id,
                };
                _context.Add(customer);
                await _context.SaveChangesAsync();

                if (request.Emails != null)
                {
                    foreach (var email in request.Emails)
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            var userCustomer = new UserCustomer
                            {
                                UserId = user.Id,
                                CustomerId = customer.Id
                            };
                            _context.UserCustomers.Add(userCustomer);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                StatusMessage = "Tạo khách hàng mới thành công!";

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            List<string> emails = await _context.Users.Select(r => r.Email).ToListAsync();
            ViewBag.allEmails = new SelectList(emails);

            var assignedEmails = await _context.UserCustomers
                .Where(ud => ud.CustomerId == id)
                .Select(ud => ud.User.Email)
                .ToListAsync();

            var customerEdit = new EditCustomerRequest
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                Address = customer.Address,
                ProvinceOrCity = customer.ProvinceOrCity,
                CompanyPhoneNumber = customer.CompanyPhoneNumber,
                ContactPersonName = customer.ContactPersonName,
                Position = customer.Position,
                ContactPersonPhoneNumber = customer.ContactPersonPhoneNumber,
                Email = customer.Email,
                BidPackageName = customer.BidPackageName,
                ProjectValue = customer.ProjectValue,
                StartDate = customer.StartDate,
                EndDate = customer.EndDate,
                IsComplete = customer.IsComplete,
                OpportunitySource = customer.OpportunitySource,
                Notes = customer.Notes,
                Emails = assignedEmails.ToArray(),
            };

            return View(customerEdit);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditCustomerRequest request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            if (ModelState.IsValid)
            {
                customer.CompanyName = request.CompanyName;
                customer.Address = request.Address;
                customer.ProvinceOrCity = request.ProvinceOrCity;
                customer.CompanyPhoneNumber = request.CompanyPhoneNumber;
                customer.ContactPersonName = request.ContactPersonName;
                customer.Position = request.Position;
                customer.ContactPersonPhoneNumber = request.ContactPersonPhoneNumber;
                customer.Email = request.Email;
                customer.BidPackageName = request.BidPackageName;
                customer.ProjectValue = request.ProjectValue;
                customer.StartDate = request.StartDate;
                customer.EndDate = request.EndDate;
                customer.IsComplete = request.IsComplete;
                customer.OpportunitySource = request.OpportunitySource;
                customer.Notes = request.Notes;
                customer.UpdatedDate = DateTime.Now;
                customer.UserId = currentUser.Id;

                _context.Update(customer);
                await _context.SaveChangesAsync();

                _context.UserCustomers.RemoveRange(_context.UserCustomers.Where(ud => ud.CustomerId == id));

                if (request.Emails != null)
                {
                    foreach (var email in request.Emails)
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            var userCustomer = new UserCustomer
                            {
                                UserId = user.Id,
                                CustomerId = customer.Id
                            };
                            _context.UserCustomers.Add(userCustomer);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật khách hàng thành công!";

                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.BidPackages)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            var customerViewModel = new CustomerViewModel
            {
                Customer = customer,
                UserCustomers = _context.UserCustomers.Include(s => s.User).Where(ud => ud.CustomerId == customer.Id).ToList()
            };

            return View(customerViewModel);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'DataContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();

            StatusMessage = "Xóa khách hàng thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(Guid id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> IndexPrivate(int? page, string searchString)
        {
            var user = await GetCurrentUserAsync();

            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 10; // Số lượng item trên mỗi trang

            var customers = from s in _context.Customers
                            where s.UserId == user.Id
                            select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(s => s.CompanyName.Contains(searchString)
                || s.Address.Contains(searchString)
                || s.ProvinceOrCity.Contains(searchString)
                || s.CompanyPhoneNumber.Contains(searchString)
                || s.ContactPersonName.Contains(searchString)
                || s.Position.Contains(searchString)
                || s.ContactPersonPhoneNumber.Contains(searchString)
                || s.Email.Contains(searchString)
                || s.BidPackageName.Contains(searchString)
                || s.ProjectValue.Contains(searchString)
                || s.OpportunitySource.Contains(searchString)
                || s.Notes.Contains(searchString)
                || s.Feedback.Contains(searchString));
            }

            customers = customers.OrderByDescending(s => s.CreatedDate);

            var pagedList = await customers.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            var customerViewModels = pagedList.Select(customer => new CustomerViewModel
            {
                Customer = customer,
                UserCustomers = _context.UserCustomers.Include(s => s.User).Where(ud => ud.CustomerId == customer.Id).ToList()
            });

            return View(customerViewModels);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> DetailsPrivate(Guid? id)
        {
            var user = await GetCurrentUserAsync();

            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Where(c => c.UserId == user.Id).Include(c => c.BidPackages)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            var customerViewModel = new CustomerViewModel
            {
                Customer = customer,
                UserCustomers = _context.UserCustomers.Include(s => s.User).Where(ud => ud.CustomerId == customer.Id).ToList()
            };

            return View(customerViewModel);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpGet]
        public async Task<IActionResult> CreatePrivate()
        {
            List<string> emails = await _context.Users.Select(r => r.Email).ToListAsync();
            ViewBag.allEmails = new SelectList(emails);

            var currentUser = await GetCurrentUserAsync();
            if (currentUser != null)
            {
                ViewData["Username"] = currentUser.Email;
            }

            return View();
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrivate(CreateCustomerRequest request)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                var customer = new Customer
                {
                    CompanyName = request.CompanyName,
                    Address = request.Address,
                    ProvinceOrCity = request.ProvinceOrCity,
                    CompanyPhoneNumber = request.CompanyPhoneNumber,
                    ContactPersonName = request.ContactPersonName,
                    Position = request.Position,
                    ContactPersonPhoneNumber = request.ContactPersonPhoneNumber,
                    Email = request.Email,
                    BidPackageName = request.BidPackageName,
                    ProjectValue = request.ProjectValue,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    OpportunitySource = request.OpportunitySource,
                    Notes = request.Notes,
                    IsComplete = false,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UserId = currentUser.Id,
                };
                _context.Add(customer);
                await _context.SaveChangesAsync();

                if (request.Emails != null)
                {
                    foreach (var email in request.Emails)
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            var userCustomer = new UserCustomer
                            {
                                UserId = user.Id,
                                CustomerId = customer.Id
                            };
                            _context.UserCustomers.Add(userCustomer);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                StatusMessage = "Tạo khách hàng mới thành công!";

                return RedirectToAction(nameof(IndexPrivate));
            }

            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> EditPrivate(Guid? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var userCurrent = await GetCurrentUserAsync();
            var customer = await _context.Customers.Where(c => c.UserId == userCurrent.Id)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            List<string> emails = await _context.Users.Select(r => r.Email).ToListAsync();
            ViewBag.allEmails = new SelectList(emails);

            var assignedEmails = await _context.UserCustomers
                .Where(ud => ud.CustomerId == id)
                .Select(ud => ud.User.Email)
                .ToListAsync();

            var customerEdit = new EditCustomerRequest
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                Address = customer.Address,
                ProvinceOrCity = customer.ProvinceOrCity,
                CompanyPhoneNumber = customer.CompanyPhoneNumber,
                ContactPersonName = customer.ContactPersonName,
                Position = customer.Position,
                ContactPersonPhoneNumber = customer.ContactPersonPhoneNumber,
                Email = customer.Email,
                BidPackageName = customer.BidPackageName,
                ProjectValue = customer.ProjectValue,
                StartDate = customer.StartDate,
                EndDate = customer.EndDate,
                IsComplete = customer.IsComplete,
                OpportunitySource = customer.OpportunitySource,
                Notes = customer.Notes,
                Emails = assignedEmails.ToArray(),
            };

            return View(customerEdit);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPrivate(Guid id, EditCustomerRequest request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }
            var userCurrent = await GetCurrentUserAsync();
            var customer = await _context.Customers.Where(c => c.UserId == userCurrent.Id)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                customer.CompanyName = request.CompanyName;
                customer.Address = request.Address;
                customer.ProvinceOrCity = request.ProvinceOrCity;
                customer.CompanyPhoneNumber = request.CompanyPhoneNumber;
                customer.ContactPersonName = request.ContactPersonName;
                customer.Position = request.Position;
                customer.ContactPersonPhoneNumber = request.ContactPersonPhoneNumber;
                customer.Email = request.Email;
                customer.BidPackageName = request.BidPackageName;
                customer.ProjectValue = request.ProjectValue;
                customer.StartDate = request.StartDate;
                customer.EndDate = request.EndDate;
                customer.IsComplete = request.IsComplete;
                customer.OpportunitySource = request.OpportunitySource;
                customer.Notes = request.Notes;
                customer.UpdatedDate = DateTime.Now;

                _context.Update(customer);
                await _context.SaveChangesAsync();

                _context.UserCustomers.RemoveRange(_context.UserCustomers.Where(ud => ud.CustomerId == id));

                if (request.Emails != null)
                {
                    foreach (var email in request.Emails)
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            var userCustomer = new UserCustomer
                            {
                                UserId = user.Id,
                                CustomerId = customer.Id
                            };
                            _context.UserCustomers.Add(userCustomer);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật khách hàng thành công!";

                return RedirectToAction(nameof(IndexPrivate));
            }
            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> DeletePrivate(Guid? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }
            var userCurrent = await GetCurrentUserAsync();
            var customer = await _context.Customers.Where(c => c.UserId == userCurrent.Id)
                .Include(c => c.BidPackages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerViewModel = new CustomerViewModel
            {
                Customer = customer,
                UserCustomers = _context.UserCustomers.Include(s => s.User).Where(ud => ud.CustomerId == customer.Id).ToList()
            };

            return View(customerViewModel);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpPost, ActionName("DeletePrivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePrivateConfirmed(Guid id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'DataContext.Customers'  is null.");
            }
            var userCurrent = await GetCurrentUserAsync();
            var customer = await _context.Customers.Where(c => c.UserId == userCurrent.Id)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();

            StatusMessage = "Xóa khách hàng thành công!";

            return RedirectToAction(nameof(IndexPrivate));
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> ListCustomers(int? page, string searchString)
        {
            var user = await GetCurrentUserAsync();

            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 10; // Số lượng item trên mỗi trang

            var customers = from s in _context.Customers
                            join uc in _context.UserCustomers on s.Id equals uc.CustomerId
                            where uc.UserId == user.Id
                            select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(s => s.CompanyName.Contains(searchString)
                || s.Address.Contains(searchString)
                || s.ProvinceOrCity.Contains(searchString)
                || s.CompanyPhoneNumber.Contains(searchString)
                || s.ContactPersonName.Contains(searchString)
                || s.Position.Contains(searchString)
                || s.ContactPersonPhoneNumber.Contains(searchString)
                || s.Email.Contains(searchString)
                || s.BidPackageName.Contains(searchString)
                || s.ProjectValue.Contains(searchString)
                || s.OpportunitySource.Contains(searchString)
                || s.Notes.Contains(searchString)
                || s.Feedback.Contains(searchString));
            }

            customers = customers.OrderByDescending(s => s.CreatedDate);

            var pagedList = await customers.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            var customerViewModels = pagedList.Select(customer => new CustomerViewModel
            {
                Customer = customer,
                UserCustomers = _context.UserCustomers.Include(s => s.User).Where(ud => ud.CustomerId == customer.Id).ToList()
            });

            return View(customerViewModels);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> DetailsAssigment(Guid? id)
        {
            var user = await GetCurrentUserAsync();

            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Include(c => c.BidPackages)
                .FirstOrDefaultAsync(m => m.Id == id && _context.UserCustomers.Any(uc => uc.CustomerId == m.Id && uc.UserId == user.Id));

            if (customer == null)
            {
                return NotFound();
            }

            var customerViewModel = new CustomerViewModel
            {
                Customer = customer,
                UserCustomers = _context.UserCustomers.Include(s => s.User).Where(ud => ud.CustomerId == customer.Id).ToList()
            };

            return View(customerViewModel);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> EditIsComplete(Guid? id)
        {
            var user = await GetCurrentUserAsync();

            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (user == null || !_context.UserCustomers.Any(ud => ud.UserId == user.Id && ud.CustomerId == id))
            {
                return Forbid();
            }

            return View(customer);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditIsComplete(Guid id, EditIsCompleteCustomerRequest request)
        {
            var user = await GetCurrentUserAsync();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (user == null || !_context.UserCustomers.Any(ud => ud.UserId == user.Id && ud.CustomerId == id))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                customer.IsComplete = request.IsComplete;
                customer.Feedback = request.Feedback;

                _context.Update(customer);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật tiến độ thành công!";

                return RedirectToAction(nameof(ListCustomers));
            }

            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> DetailsBidPackage(Guid? id)
        {
            if (id == null || _context.BidPackages == null)
            {
                return NotFound();
            }

            var bidPackage = await _context.BidPackages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bidPackage == null)
            {
                return NotFound();
            }

            return View(bidPackage);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpGet]
        public async Task<IActionResult> CreateBidPackage(Guid customerId)
        {
            var model = new CreateBidPackageRequest
            {
                CustomerId = customerId
            };
            return View(model);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBidPackage(CreateBidPackageRequest request)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                var bidPackage = new BidPackage
                {
                    Name = request.Name,
                    Notes = request.Notes,
                    CustomerId = request.CustomerId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UserId = currentUser.Id,
                };
                _context.BidPackages.Add(bidPackage);
                await _context.SaveChangesAsync();

                StatusMessage = "Tạo gối thầu mới thành công!";

                return RedirectToAction(nameof(Details), new { id = request.CustomerId });
            }
            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        public async Task<IActionResult> EditBidPackage(Guid id)
        {
            var bidPackage = await _context.BidPackages.FindAsync(id);
            if (bidPackage == null)
            {
                return NotFound();
            }
            var model = new EditBidPackageRequest
            {
                Id = bidPackage.Id,
                Name = bidPackage.Name,
                Notes = bidPackage.Notes,
                CustomerId = bidPackage.CustomerId,
            };
            return View(model);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBidPackage(EditBidPackageRequest request)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                var bidPackage = await _context.BidPackages.FindAsync(request.Id);
                if (bidPackage == null)
                {
                    return NotFound();
                }
                bidPackage.Name = request.Name;
                bidPackage.Notes = request.Notes;
                bidPackage.UpdatedDate = DateTime.Now;
                bidPackage.UserId = currentUser.Id;
                _context.BidPackages.Update(bidPackage);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật gối thầu mới thành công!";

                return RedirectToAction(nameof(Details), new { id = request.CustomerId });
            }
            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBidPackage(Guid id)
        {
            var bidPackage = await _context.BidPackages.FindAsync(id);
            if (bidPackage == null)
            {
                return NotFound();
            }
            _context.BidPackages.Remove(bidPackage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = bidPackage.CustomerId });
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> DetailsBidPackagePrivate(Guid? id)
        {
            var user = await GetCurrentUserAsync();

            if (id == null || _context.BidPackages == null)
            {
                return NotFound();
            }

            var bidPackage = await _context.BidPackages
                .Where(c => c.UserId == user.Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bidPackage == null)
            {
                return NotFound();
            }

            return View(bidPackage);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpGet]
        public async Task<IActionResult> CreateBidPackagePrivate(Guid customerId)
        {
            var model = new CreateBidPackageRequest
            {
                CustomerId = customerId
            };
            return View(model);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBidPackagePrivate(CreateBidPackageRequest request)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                var bidPackage = new BidPackage
                {
                    Name = request.Name,
                    Notes = request.Notes,
                    CustomerId = request.CustomerId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UserId = currentUser.Id,
                };
                _context.BidPackages.Add(bidPackage);
                await _context.SaveChangesAsync();

                StatusMessage = "Tạo gối thầu mới thành công!";

                return RedirectToAction(nameof(DetailsPrivate), new { id = request.CustomerId });
            }
            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        public async Task<IActionResult> EditBidPackagePrivate(Guid id)
        {
            var userCurrent = await GetCurrentUserAsync();
            var bidPackage = await _context.BidPackages.Where(c => c.UserId == userCurrent.Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bidPackage == null)
            {
                return NotFound();
            }
            var model = new EditBidPackageRequest
            {
                Id = bidPackage.Id,
                Name = bidPackage.Name,
                Notes = bidPackage.Notes,
                CustomerId = bidPackage.CustomerId,
            };
            return View(model);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBidPackagePrivate(EditBidPackageRequest request)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                var bidPackage = await _context.BidPackages.Where(c => c.UserId == currentUser.Id)
                .FirstOrDefaultAsync(m => m.Id == request.Id);
                if (bidPackage == null)
                {
                    return NotFound();
                }
                bidPackage.Name = request.Name;
                bidPackage.Notes = request.Notes;
                bidPackage.UpdatedDate = DateTime.Now;
                bidPackage.UserId = currentUser.Id;
                _context.BidPackages.Update(bidPackage);
                await _context.SaveChangesAsync();

                StatusMessage = "Cập nhật gối thầu mới thành công!";

                return RedirectToAction(nameof(DetailsPrivate), new { id = request.CustomerId });
            }
            return View(request);
        }

        [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBidPackagePrivate(Guid id)
        {
            var currentUser = await GetCurrentUserAsync();
            var bidPackage = await _context.BidPackages.Where(c => c.UserId == currentUser.Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bidPackage == null)
            {
                return NotFound();
            }
            _context.BidPackages.Remove(bidPackage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DetailsPrivate), new { id = bidPackage.CustomerId });
        }
    }
}
