using System.Threading.Tasks;
using SchoolApp.Models;

namespace SchoolApp.Services
{
    public interface IPaymobService
    {
        Task<(string? paymentKey, string? orderId)> GetPaymentKeyAsync(Course course, Trainee trainee);
        bool ValidateHMAC(Microsoft.AspNetCore.Http.IQueryCollection query);
    }
}
