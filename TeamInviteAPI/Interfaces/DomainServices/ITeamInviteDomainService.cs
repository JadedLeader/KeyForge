using KeyForgedShared.DTO_s.TeamInviteDTO_s;
using KeyForgedShared.SharedDataModels;
using KeyForgedShared.ValidationType;

namespace TeamInviteAPI.Interfaces.DomainServices
{
    public interface ITeamInviteDomainService
    {
        Task<TeamInviteDataModel> CreateTeamInvite(TeamInviteDataModel teamInvite);
        Task<CreateInviteValidationResult> ValidateInviteCreation(CreateTeamInviteDto teamInvite, Guid senderAccountId);

        Task<GetPendingInvitesValidationResult> ValidatePendingInvites(GetCurrentPendingTeamInvitesDto pendingTeamInvites, Guid accountId);

        Task<bool> ValidateTeamInviteRejection(RejectTeamInviteDto rejectTeamInvite, Guid accountId);

        Task<TeamInviteDataModel> DeleteTeamInvite(Guid teamInviteId);
    }
}