using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SchoolApp.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
