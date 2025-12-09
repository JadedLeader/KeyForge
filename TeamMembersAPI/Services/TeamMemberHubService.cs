using KeyForgedShared.HubProjection.TeamMember;
using Microsoft.AspNetCore.SignalR;
using TeamMembersAPI.Hubs;
using TeamMembersAPI.Interfaces.Services;

namespace TeamMembersAPI.Services
{
    public class TeamMemberHubService : ITeamMemberHubService
    {

        private readonly IHubContext<TeamMemberHub> _hubContext;

        public TeamMemberHubService(IHubContext<TeamMemberHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyTeamMemberAdded(string teamVaultId, TeamMemberAddedProjection teamMemberAdded)
        {
            await _hubContext.Clients.Group($"Vault-{teamVaultId}").SendAsync($"TeamMemberAdded", teamMemberAdded);
        }

    }
}
