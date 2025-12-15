using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace InternMS.Api.Hubs
{
    [Authorize] // require authenticated users
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public Task Ping() => Clients.Caller.SendAsync("Pong");
    }
}