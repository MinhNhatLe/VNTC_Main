﻿using dotnetstartermvc.Data;
using dotnetstartermvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using ContactModel = dotnetstartermvc.Models.Contacts.Contact;

namespace dotnetstartermvc.Areas.Contact.Controllers
{
    [Area("Contact")]
    [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator}")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [TempData]
        public string StatusMessage { set; get; }

        [HttpGet("/admin/contact")]
        public async Task<IActionResult> Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1; // Trang hiện tại
            var pageSize = 10; // Số lượng item trên mỗi trang

            var contact = from u in _context.Contacts select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                contact = contact.Where(u => u.FullName.Contains(searchString)
                || u.Email.Contains(searchString)
                || u.Phone.Contains(searchString));
            }

            contact = contact.OrderByDescending(u => u.DateSent);
            var pagedList = await contact.ToPagedListAsync(pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;

            return View(pagedList);
        }

        [HttpGet("/admin/contact/detail/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        [AllowAnonymous]
        [HttpGet("/contact/")]
        public IActionResult SendContact()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("/contact/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendContact([Bind("FullName,Email,Message,Phone")] ContactModel contact)
        {
            if (ModelState.IsValid)
            {
                contact.DateSent = DateTime.Now;

                _context.Add(contact);
                await _context.SaveChangesAsync();

                StatusMessage = "Liên hệ của bạn đã được gửi thành công!";

                return RedirectToAction("", "Contact");
                //return View(contact);
            }

            return View(contact);
        }

        [HttpGet("/admin/contact/delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        [HttpPost("/admin/contact/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            StatusMessage = "Bạn vừa xóa thành công liên hệ người dùng!";

            return RedirectToAction(nameof(Index));
        }
    }
}