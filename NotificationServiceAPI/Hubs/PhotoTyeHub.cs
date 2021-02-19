using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NotificationServiceAPI.Hubs
{
    public class PhotoTyeHub : Hub
    {
        public Task JoinGroup(string groupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}

