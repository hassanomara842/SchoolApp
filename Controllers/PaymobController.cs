using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using SchoolApp.Services;
using System.Threading.Tasks;
using System.Linq;

namespace SchoolApp.Controllers
{
    public class PaymobController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPaymobService _paymobService;

        public PaymobController(AppDbContext context, IPaymobService paymobService)
        {
            _context = context;
            _paymobService = paymobService;
        }

        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // Paymob sends data via query parameters for synchronous redirect callback
            var query = Request.Query;

            bool isValid = _paymobService.ValidateHMAC(query);
            if (!isValid)
            {
                TempData["InfoMessage"] = "Payment verification failed. Security signature mismatch.";
                return RedirectToAction("Index", "Home");
            }

            bool success = query["success"].ToString().ToLower() == "true";
            string orderId = query["order"].ToString();
            string transactionId = query["id"].ToString();

            var enrollment = await _context.Enrollments.Include(e => e.Course).FirstOrDefaultAsync(e => e.PaymobOrderId == orderId);

            if (enrollment == null)
            {
                TempData["InfoMessage"] = "Order not found.";
                return RedirectToAction("Index", "Home");
            }

            if (success)
            {
                enrollment.IsPaid = true;
                enrollment.TransactionId = transactionId;
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Payment successful! Welcome to {enrollment.Course?.Name}.";
                return RedirectToAction("Details", "Course", new { id = enrollment.CourseId });
            }
            else
            {
                // Payment failed
                TempData["InfoMessage"] = "Payment failed or was cancelled. Please try again.";
                return RedirectToAction("Details", "Course", new { id = enrollment.CourseId });
            }
        }
    }
}
