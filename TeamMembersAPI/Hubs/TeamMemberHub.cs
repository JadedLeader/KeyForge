using Microsoft.AspNetCore.SignalR;


namespace TeamMembersAPI.Hubs
{
    public class TeamMemberHub : Hub
    {

        public async Task JoinVaultGroup(string teamVaultId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Vault-{teamVaultId}");
        }

    }
}
