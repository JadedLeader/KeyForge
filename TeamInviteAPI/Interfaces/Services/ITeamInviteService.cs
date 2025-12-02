using KeyForgedShared.DTO_s.TeamInviteDTO_s;
using KeyForgedShared.ReturnTypes.TeamInvite;

namespace TeamInviteAPI.Interfaces.Services
{
    public interface ITeamInviteService
    {
        Task<CreateTeamInviteReturn> CreateTeamInvite(CreateTeamInviteDto teamInvite, string shortLivedToken);
        Task RejectAllTeamInvites(string shortLivedToken);

        Task<GetCurrentPendingTeamInvitesReturn> GetCurrentPendingTeamInvites(GetCurrentPendingTeamInvitesDto getCurrentPendingInvites, string shortLivedToken);

        Task<RejectTeamInviteReturn> RejectTeamInvite(RejectTeamInviteDto rejectTeamInvite, string shortLivedToken);

        Task<UpdateTeamInviteReturn> UpdateTeamInvite(UpdateTeamInviteDto updateTeamInvite, string shortLivedToken);

        Task<GetAllPendingInvitesForAccountReturn> GetAllPendingInvitesForAccount(GetAllPendingInvitesForAccountDto getPendingInvites, string shortLivedToken);
    }
}