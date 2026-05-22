using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SchoolApp.Hubs;
using System.IO;

namespace SchoolApp.Controllers
{
    [Authorize(Roles = "Admin,Instructor")]
    public class AttendanceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendanceController(AppDbContext context, IHubContext<NotificationHub> hubContext, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Export(int courseId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) return NotFound();

            var attendances = await _context.Attendances
                .Include(a => a.Trainee)
                .Where(a => a.CourseId == courseId)
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Trainee!.Name)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance Report");

                // Headers
                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Trainee Name";
                worksheet.Cell(1, 3).Value = "Status";
                worksheet.Cell(1, 4).Value = "Remarks";

                worksheet.Range("A1:D1").Style.Font.Bold = true;
                worksheet.Range("A1:D1").Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var a in attendances)
                {
                    worksheet.Cell(row, 1).Value = a.Date.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 2).Value = a.Trainee?.Name;
                    worksheet.Cell(row, 3).Value = a.IsPresent ? "Present" : "Absent";
                    worksheet.Cell(row, 4).Value = a.Remarks;

                    if (a.IsPresent)
                        worksheet.Cell(row, 3).Style.Font.FontColor = XLColor.Green;
                    else
                        worksheet.Cell(row, 3).Style.Font.FontColor = XLColor.Red;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Attendance_{course.Name.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Take(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return NotFound();

            var today = DateTime.UtcNow.Date;

            // Fetch all trainees in this course's department
            var trainees = await _context.Trainees
                .Where(t => t.Dept_id == course.Dept_id)
                .ToListAsync();

            // Fetch any existing attendance for today
            var existingAttendance = await _context.Attendances
                .Where(a => a.CourseId == courseId && a.Date.Date == today)
                .ToListAsync();

            var viewModel = new TakeAttendanceViewModel
            {
                CourseId = courseId,
                CourseName = course.Name,
                Date = today,
                Records = trainees.Select(t => new AttendanceRecord
                {
                    TraineeId = t.Id,
                    TraineeName = t.Name,
                    IsPresent = existingAttendance.FirstOrDefault(a => a.TraineeId == t.Id)?.IsPresent ?? false,
                    Remarks = existingAttendance.FirstOrDefault(a => a.TraineeId == t.Id)?.Remarks
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Take(TakeAttendanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var today = model.Date.Date;
            var course = await _context.Courses.FindAsync(model.CourseId);

            // Get existing attendance
            var existingAttendance = await _context.Attendances
                .Where(a => a.CourseId == model.CourseId && a.Date.Date == today)
                .ToListAsync();

            foreach (var record in model.Records)
            {
                var existing = existingAttendance.FirstOrDefault(a => a.TraineeId == record.TraineeId);
                bool wasPresent = true; // Assume present if new

                if (existing != null)
                {
                    wasPresent = existing.IsPresent;
                    existing.IsPresent = record.IsPresent;
                    existing.Remarks = record.Remarks;
                }
                else
                {
                    _context.Attendances.Add(new Attendance
                    {
                        CourseId = model.CourseId,
                        TraineeId = record.TraineeId,
                        Date = today,
                        IsPresent = record.IsPresent,
                        Remarks = record.Remarks
                    });
                }

                // If marked absent (and wasn't already absent previously today)
                if (!record.IsPresent && (existing == null || wasPresent))
                {
                    // Find user by name
                    var traineeUser = await _userManager.Users.FirstOrDefaultAsync(u => u.FullName == record.TraineeName);
                    if (traineeUser != null)
                    {
                        var msg = $"You have been marked absent for {course?.Name} on {today:yyyy-MM-dd}.";
                        _context.Notifications.Add(new Notification { UserId = traineeUser.Id, Message = msg });
                        
                        // Using SendAsync to All for simplicity since mapping User IDs in SignalR needs setup
                        // In a real app we'd map Connections to Users.
                        await _hubContext.Clients.User(traineeUser.Id).SendAsync("ReceiveNotification", msg);
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Attendance saved successfully!";
            return RedirectToAction("Take", new { courseId = model.CourseId });
        }

        [HttpGet]
        public async Task<IActionResult> GenerateQR(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            ViewBag.CourseId = courseId;
            ViewBag.CourseName = course.Name;
            ViewBag.Date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ScanQR(int courseId, string date)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Attendance/ScanQR?courseId={courseId}&date={date}" });
            }

            if (!User.IsInRole("Trainee"))
            {
                return Content("Only trainees can scan this QR code.");
            }

            var traineeUser = await _userManager.GetUserAsync(User);
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.Name == traineeUser.FullName);

            if (trainee == null) return Content("Trainee record not found.");

            if (!DateTime.TryParse(date, out DateTime scanDate))
            {
                return Content("Invalid date.");
            }

            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a => a.CourseId == courseId && a.TraineeId == trainee.Id && a.Date.Date == scanDate.Date);

            if (existing != null)
            {
                existing.IsPresent = true;
                existing.Remarks = "QR Scanned";
            }
            else
            {
                _context.Attendances.Add(new Attendance
                {
                    CourseId = courseId,
                    TraineeId = trainee.Id,
                    Date = scanDate.Date,
                    IsPresent = true,
                    Remarks = "QR Scanned"
                });
            }

            await _context.SaveChangesAsync();
            
            return View("QRSuccess");
        }
    }
}
