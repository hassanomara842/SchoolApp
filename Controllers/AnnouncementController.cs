using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;

namespace SchoolApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AnnouncementController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AnnouncementController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Announcement
        public async Task<IActionResult> Index()
        {
            return View(await _context.Announcements.OrderByDescending(a => a.CreatedAt).ToListAsync());
        }

        // GET: Announcement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Announcement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ImageUrl,IsActive")] Announcement announcement, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string uploads = Path.Combine(wwwRootPath, @"images\announcements");
                    
                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    announcement.ImageUrl = @"/images/announcements/" + fileName;
                }
                else if (string.IsNullOrEmpty(announcement.ImageUrl))
                {
                    announcement.ImageUrl = "https://images.unsplash.com/photo-1540317580384-e5d43616b9aa?q=80&w=600&auto=format&fit=crop";
                }
                
                announcement.CreatedAt = System.DateTime.UtcNow;
                _context.Add(announcement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(announcement);
        }

        // GET: Announcement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            return View(announcement);
        }

        // POST: Announcement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ImageUrl,CreatedAt,IsActive")] Announcement announcement, IFormFile? imageFile)
        {
            if (id != announcement.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string uploads = Path.Combine(wwwRootPath, @"images\announcements");
                        
                        if (!Directory.Exists(uploads))
                            Directory.CreateDirectory(uploads);

                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        announcement.ImageUrl = @"/images/announcements/" + fileName;
                    }

                    _context.Update(announcement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnouncementExists(announcement.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(announcement);
        }

        // POST: Announcement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnnouncementExists(int id)
        {
            return _context.Announcements.Any(e => e.Id == id);
        }
    }
}
