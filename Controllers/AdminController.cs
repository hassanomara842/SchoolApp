using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using SchoolApp.Models;

namespace SchoolApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var announcements = await _context.Announcements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();
            
            ViewBag.Announcements = announcements;

            var model = new DashboardViewModel
            {
                TotalStudents = _context.Trainees.Count(),
                TotalInstructors = _context.Instructors.Count(),
                TotalCourses = _context.Courses.Count(),
                TotalDepartments = _context.Departments.Count(),
                DepartmentStats = _context.Departments.Select(d => new DepartmentStats
                {
                    DepartmentName = d.Name,
                    StudentCount = d.Trainees != null ? d.Trainees.Count : 0
                }).ToList(),
                CourseStats = _context.Courses.Select(c => new CourseStats
                {
                    CourseName = c.Name,
                    EnrollmentCount = _context.CrsResults.Count(cr => cr.Crs_id == c.Id)
                }).ToList()
            };
            return View(model);
        }

        public async Task<IActionResult> ExportTraineesToExcel()
        {
            var trainees = await _context.Trainees
                .Include(t => t.Department)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Trainees");
                
                // Headers
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Address";
                worksheet.Cell(1, 4).Value = "Department";
                worksheet.Cell(1, 5).Value = "Overall Grade";
                
                // Styling headers
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                for (int i = 0; i < trainees.Count; i++)
                {
                    var t = trainees[i];
                    int row = i + 2;
                    worksheet.Cell(row, 1).Value = t.Id;
                    worksheet.Cell(row, 2).Value = t.Name;
                    worksheet.Cell(row, 3).Value = t.Address;
                    worksheet.Cell(row, 4).Value = t.Department?.Name ?? "N/A";
                    worksheet.Cell(row, 5).Value = t.Grade;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Trainees_Report.xlsx");
                }
            }
        }
    }
}
