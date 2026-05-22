using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using SchoolApp.Services;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SchoolApp.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace SchoolApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IPaymobService _paymobService;

        public CourseController(AppDbContext context, IHubContext<NotificationHub> hubContext, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment, IPaymobService paymobService)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _paymobService = paymobService;
        }

        // GET: Course
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var courses = _context.Courses.Include(c => c.Department);
            return View(await courses.ToListAsync());
        }

        // GET: Course/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Degree,MinDegree,Hrs,Price,Dept_id,Description,ImageUrl,DiscountPrice,BadgeText,BadgeColor")] Course course, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string uploads = Path.Combine(wwwRootPath, @"images\courses");
                    
                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    course.ImageUrl = @"/images/courses/" + fileName;
                }

                _context.Add(course);
                await _context.SaveChangesAsync();

                // Notify all trainees about the new course
                var trainees = await _userManager.GetUsersInRoleAsync("Trainee");
                foreach (var trainee in trainees)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = trainee.Id,
                        Message = $"A new course has been added: {course.Name}!"
                    });
                }
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"A new course has been added: {course.Name}!");

                return RedirectToAction(nameof(Index));
            }
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", course.Dept_id);
            return View(course);
        }

        // GET: Course/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", course.Dept_id);
            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Degree,MinDegree,Hrs,Price,Dept_id,Description,ImageUrl,DiscountPrice,BadgeText,BadgeColor")] Course course, IFormFile? imageFile)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string uploads = Path.Combine(wwwRootPath, @"images\courses");
                        
                        if (!Directory.Exists(uploads))
                            Directory.CreateDirectory(uploads);

                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        course.ImageUrl = @"/images/courses/" + fileName;
                    }

                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            ViewData["Dept_id"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", course.Dept_id);
            return View(course);
        }

        // GET: Course/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.CourseMaterials)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            if (User.IsInRole("Trainee"))
            {
                var traineeUser = await _userManager.GetUserAsync(User);
                var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.Name == traineeUser.FullName);
                if (trainee != null)
                {
                    ViewBag.IsEnrolled = await _context.Enrollments.AnyAsync(e => e.CourseId == id && e.TraineeId == trainee.Id && e.IsPaid);
                }
                else
                {
                    ViewBag.IsEnrolled = false;
                }
            }

            return View(course);
        }

        // GET: Course/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Discussion(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();

            var messages = await _context.ChatMessages
                .Include(m => m.User)
                .Where(m => m.CourseId == id)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            ViewBag.CourseName = course.Name;
            ViewBag.CourseId = course.Id;
            return View(messages);
        }

        [Authorize(Roles = "Trainee")]
        [HttpGet]
        public async Task<IActionResult> Checkout(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var traineeUser = await _userManager.GetUserAsync(User);
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.Name == traineeUser.FullName);

            if (trainee == null) return RedirectToAction("Index", "Home");

            // Check if already enrolled
            var existing = await _context.Enrollments.FirstOrDefaultAsync(e => e.CourseId == id && e.TraineeId == trainee.Id);
            if (existing != null)
            {
                TempData["InfoMessage"] = "You are already enrolled in this course.";
                return RedirectToAction("Details", new { id = course.Id });
            }

            return View(course);
        }

        [Authorize(Roles = "Trainee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutConfirm(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            var traineeUser = await _userManager.GetUserAsync(User);
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.Name == traineeUser.FullName);

            if (trainee != null)
            {
                var enrollment = new Enrollment
                {
                    CourseId = courseId,
                    TraineeId = trainee.Id,
                    IsPaid = false // Pending Paymob callback
                };
                
                var (paymentKey, orderId) = await _paymobService.GetPaymentKeyAsync(course, trainee);
                
                if (paymentKey != null)
                {
                    enrollment.PaymobOrderId = orderId;
                    _context.Enrollments.Add(enrollment);
                    await _context.SaveChangesAsync();
                    
                    var config = HttpContext.RequestServices.GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration)) as Microsoft.Extensions.Configuration.IConfiguration;
                    var iframeId = config?["PaymobSettings:IframeId"];
                    
                    return Redirect($"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}");
                }
                else
                {
                    TempData["InfoMessage"] = "Payment gateway error. Please try again later.";
                }
            }

            return RedirectToAction("Details", new { id = courseId });
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
