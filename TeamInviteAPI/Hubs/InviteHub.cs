using Microsoft.AspNetCore.SignalR;

namespace TeamInviteAPI.Hubs
{
    public class InviteHub : Hub
    {

        public async Task Register(string userIdentifier)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userIdentifier);
        }

    }
}
