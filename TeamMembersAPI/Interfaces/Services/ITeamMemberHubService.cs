using KeyForgedShared.HubProjection.TeamMember;

namespace TeamMembersAPI.Interfaces.Services
{
    public interface ITeamMemberHubService
    {
        Task NotifyTeamMemberAdded(string teamVaultId, TeamMemberAddedProjection teamMemberAdded);
    }
}