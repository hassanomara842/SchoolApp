using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;

namespace SchoolApp.Controllers
{
    [Authorize]
    public class CourseMaterialController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CourseMaterialController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int courseId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction("Details", "Course", new { id = courseId });
            }

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "materials");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var material = new CourseMaterial
            {
                CourseId = courseId,
                Title = Path.GetFileNameWithoutExtension(file.FileName),
                FileName = file.FileName,
                FilePath = "/uploads/materials/" + uniqueFileName,
                ContentType = file.ContentType,
                UploadDate = DateTime.UtcNow,
                UploadedByUserId = User.Identity?.Name ?? "Unknown"
            };

            _context.CourseMaterials.Add(material);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Material uploaded successfully.";
            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var material = await _context.CourseMaterials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && !User.IsInRole("Instructor"))
            {
                return Forbid();
            }

            var physicalPath = Path.Combine(_environment.WebRootPath, material.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _context.CourseMaterials.Remove(material);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Material deleted successfully.";
            return RedirectToAction("Details", "Course", new { id = material.CourseId });
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var material = await _context.CourseMaterials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            var physicalPath = Path.Combine(_environment.WebRootPath, material.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(physicalPath))
            {
                return NotFound();
            }

            return PhysicalFile(physicalPath, material.ContentType, material.FileName);
        }
    }
}
