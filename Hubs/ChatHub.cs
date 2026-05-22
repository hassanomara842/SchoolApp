using Microsoft.AspNetCore.SignalR;
using SchoolApp.Models;
using Microsoft.AspNetCore.Identity;

namespace SchoolApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task JoinCourseGroup(string courseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Course_{courseId}");
        }

        public async Task LeaveCourseGroup(string courseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Course_{courseId}");
        }

        public async Task SendMessage(string courseId, string message)
        {
            var userId = _userManager.GetUserId(Context.User);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user != null)
            {
                var chatMsg = new ChatMessage
                {
                    CourseId = int.Parse(courseId),
                    UserId = userId,
                    Message = message,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ChatMessages.Add(chatMsg);
                await _context.SaveChangesAsync();

                await Clients.Group($"Course_{courseId}").SendAsync("ReceiveMessage", user.FullName ?? user.UserName, user.ProfilePicturePath, message, chatMsg.CreatedAt.ToString("HH:mm"));
            }
        }
    }
}
