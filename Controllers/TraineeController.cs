using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace SchoolApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TraineeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostEnvironment;

        public TraineeController(AppDbContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Trainee
        public async Task<IActionResult> Index()
        {
            var trainees = _context.Trainees.Include(t => t.Department);
            return View(await trainees.ToListAsync());
        }

        // GET: Trainee/Create
        public IActionResult Create()
        {
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Trainee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image,Address,Grade,Dept_id")] Trainee trainee, Microsoft.AspNetCore.Http.IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = System.IO.Path.Combine(_hostEnvironment.WebRootPath, "images", "trainees");
                    System.IO.Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = System.Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    trainee.Image = "/images/trainees/" + uniqueFileName;
                }

                _context.Add(trainee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", trainee.Dept_id);
            return View(trainee);
        }

        // GET: Trainee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null)
            {
                return NotFound();
            }
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", trainee.Dept_id);
            return View(trainee);
        }

        // POST: Trainee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image,Address,Grade,Dept_id")] Trainee trainee, Microsoft.AspNetCore.Http.IFormFile? imageFile)
        {
            if (id != trainee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadsFolder = System.IO.Path.Combine(_hostEnvironment.WebRootPath, "images", "trainees");
                        System.IO.Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = System.Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        trainee.Image = "/images/trainees/" + uniqueFileName;
                    }

                    _context.Update(trainee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TraineeExists(trainee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", trainee.Dept_id);
            return View(trainee);
        }

        // GET: Trainee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainee = await _context.Trainees
                .Include(t => t.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }

        // POST: Trainee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainee = await _context.Trainees.FindAsync(id);
            if (trainee != null)
            {
                _context.Trainees.Remove(trainee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Leaderboard()
        {
            // Calculate XP (Grade * 10) as a simple example, assuming Grade is out of 100
            var trainees = await _context.Trainees
                .Include(t => t.Department)
                .OrderByDescending(t => t.Grade)
                .Take(10)
                .ToListAsync();

            var leaderboard = new List<LeaderboardViewModel>();
            int rank = 1;
            
            // To get profile pictures, we need to match Trainee with ApplicationUser.
            // But currently Trainee doesn't have an UserId link. We just use a default image or the one on the Trainee model if it exists.
            foreach (var t in trainees)
            {
                leaderboard.Add(new LeaderboardViewModel
                {
                    Rank = rank++,
                    TraineeId = t.Id,
                    Name = t.Name,
                    ProfilePicture = string.IsNullOrEmpty(t.Image) ? "/images/default-profile.png" : $"/images/trainees/{t.Image}",
                    DepartmentName = t.Department?.Name ?? "Unknown",
                    TotalXP = (int)(t.Grade * 10)
                });
            }

            return View(leaderboard);
        }

        private bool TraineeExists(int id)
        {
            return _context.Trainees.Any(e => e.Id == id);
        }
    }
}
