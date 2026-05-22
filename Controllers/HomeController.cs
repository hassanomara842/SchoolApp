using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SchoolApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var announcements = await _context.Announcements
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .ToListAsync();
        
        ViewBag.Announcements = announcements;

        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        if (User.IsInRole("Trainee"))
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            ViewBag.TotalCourses = _context.Courses.Count();
            ViewBag.PendingQuizzes = _context.Quizzes.Count();
            return View("TraineeDashboard");
        }
        
        if (User.IsInRole("Instructor"))
        {
            ViewBag.TotalCourses = _context.Courses.Count();
            ViewBag.TotalTrainees = _context.Trainees.Count();
            return View("InstructorDashboard");
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize]
    public IActionResult Calendar()
    {
        return View();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCalendarEvents()
    {
        var courses = await _context.Courses.ToListAsync();
        
        var events = courses.Select(c => new
        {
            title = c.Name,
            start = DateTime.UtcNow.AddDays(c.Id).ToString("yyyy-MM-dd"),
            backgroundColor = "#6366f1",
            borderColor = "#4f46e5",
            url = $"/Course/Details/{c.Id}"
        }).ToList();

        return Json(events);
    }
}
