using KeyForgedShared.Enums;
using KeyForgedShared.SharedDataModels;
using Microsoft.AspNetCore.SignalR;
using TeamInviteAPI.Hubs;
using TeamInviteAPI.Interfaces.Services;
using KeyForgedShared.DTO_s.HubDTO_s;

namespace TeamInviteAPI.Services
{
    public class SignalRInviteService : ISignalRInviteService
    {

        private readonly IHubContext<InviteHub> _hubContext;

        public SignalRInviteService(IHubContext<InviteHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushInviteToHub(TeamInviteDataModel inviteDataModel)
        {
            if (inviteDataModel.InviteStatus == InviteStatus.Pending.ToString())
            {
                await _hubContext.Clients.Group(inviteDataModel.InviteRecipient)
                    .SendAsync("ReceiveInvite", new TeamInviteDto
                    {

                        TeamInviteId = inviteDataModel.Id.ToString(), 
                        InviteSentBy = inviteDataModel.InviteSentBy,
                        InviteRecipient = inviteDataModel.InviteRecipient,
                        InviteStatus = inviteDataModel.InviteStatus,
                        InviteCreatedAt = inviteDataModel.InviteCreatedAt,

                    });
            }
        }



    }
}
