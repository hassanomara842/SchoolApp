using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace SchoolApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InstructorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostEnvironment;

        public InstructorController(AppDbContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Instructor
        public async Task<IActionResult> Index(string searchString)
        {
            var instructorsQuery = _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Course)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                instructorsQuery = instructorsQuery.Where(i => i.Name.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            return View(await instructorsQuery.ToListAsync());
        }

        // GET: Instructor/Detail/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructor/Create
        public IActionResult Create()
        {
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name");
            ViewData["Crs_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        // POST: Instructor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image,Salary,Address,Dept_id,Crs_id")] Instructor instructor, Microsoft.AspNetCore.Http.IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = System.IO.Path.Combine(_hostEnvironment.WebRootPath, "images", "instructors");
                    System.IO.Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = System.Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    instructor.Image = "/images/instructors/" + uniqueFileName;
                }

                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", instructor.Dept_id);
            ViewData["Crs_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Courses, "Id", "Name", instructor.Crs_id);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", instructor.Dept_id);
            ViewData["Crs_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Courses, "Id", "Name", instructor.Crs_id);
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image,Salary,Address,Dept_id,Crs_id")] Instructor instructor, Microsoft.AspNetCore.Http.IFormFile? imageFile)
        {
            if (id != instructor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadsFolder = System.IO.Path.Combine(_hostEnvironment.WebRootPath, "images", "instructors");
                        System.IO.Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = System.Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        instructor.Image = "/images/instructors/" + uniqueFileName;
                    }
                    
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.Id))
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
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", instructor.Dept_id);
            ViewData["Crs_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Courses, "Id", "Name", instructor.Crs_id);
            return View(instructor);
        }

        // GET: Instructor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.Id == id);
        }
    }
}
