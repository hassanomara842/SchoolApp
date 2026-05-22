using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;

namespace SchoolApp.Controllers
{
    [Authorize(Roles = "Trainee")]
    public class CertificateController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CertificateController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Generate(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            
            // Find the highest score attempt for this course
            var bestAttempt = await _context.QuizAttempts
                .Include(qa => qa.Quiz)
                .ThenInclude(q => q.Course)
                .Where(qa => qa.TraineeUserId == user.Id && qa.Quiz.CourseId == courseId)
                .OrderByDescending(qa => (double)qa.Score / qa.TotalScore)
                .FirstOrDefaultAsync();

            if (bestAttempt == null) return NotFound("You have not taken any quizzes for this course yet.");

            double percentage = ((double)bestAttempt.Score / bestAttempt.TotalScore) * 100;
            var course = bestAttempt.Quiz.Course;

            if (percentage < 50) // Assuming 50% is passing, or use course.MinDegree if it was a percentage
            {
                return BadRequest("You have not passed a quiz for this course yet. Keep trying!");
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20).FontFamily(Fonts.Arial));

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);
                            
                            x.Item().Text("CERTIFICATE OF COMPLETION")
                                .SemiBold().FontSize(40).FontColor(Colors.Blue.Darken2)
                                .AlignCenter();

                            x.Item().PaddingTop(20).Text("This is to certify that")
                                .FontSize(20).FontColor(Colors.Grey.Darken2)
                                .AlignCenter();

                            x.Item().Text(user.FullName ?? user.UserName)
                                .SemiBold().FontSize(45).FontColor(Colors.Black)
                                .AlignCenter();

                            x.Item().Text("has successfully completed the course")
                                .FontSize(20).FontColor(Colors.Grey.Darken2)
                                .AlignCenter();

                            x.Item().Text(course.Name)
                                .SemiBold().FontSize(35).FontColor(Colors.Indigo.Darken2)
                                .AlignCenter();

                            x.Item().PaddingTop(20).Text($"With a passing score of: {percentage:0.0}%")
                                .FontSize(24).FontColor(Colors.Grey.Darken3)
                                .AlignCenter();

                            x.Item().PaddingTop(40).Row(row =>
                            {
                                row.RelativeItem().Column(col => 
                                {
                                    col.Item().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).Text(DateTime.Now.ToString("MMMM dd, yyyy")).FontSize(18).AlignCenter();
                                    col.Item().Text("Date").FontSize(14).FontColor(Colors.Grey.Darken1).AlignCenter();
                                });
                                
                                row.RelativeItem().Column(col => 
                                {
                                    col.Item().Text("").FontSize(18); // Spacing
                                });

                                row.RelativeItem().Column(col => 
                                {
                                    col.Item().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).Text("SchoolApp Administration").FontSize(18).AlignCenter();
                                    col.Item().Text("Signature").FontSize(14).FontColor(Colors.Grey.Darken1).AlignCenter();
                                });
                            });
                        });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"{course.Name}_Certificate.pdf");
        }
    }
}
