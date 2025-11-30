using KeyForgedShared.DTO_s.TeamInviteDTO_s;
using KeyForgedShared.Enums;
using KeyForgedShared.Interfaces;
using KeyForgedShared.ReturnTypes.TeamInvite;
using KeyForgedShared.SharedDataModels;
using TeamInviteAPI.Interfaces.Repos;
using TeamInviteAPI.Interfaces.Services;

namespace TeamInviteAPI.Services
{
    public class TeamInviteService : ITeamInviteService
    {

        private readonly IAccountRepo _accountRepo;
        private readonly ITeamRepo _teamRepo;
        private readonly ITeamVaultRepo _teamVaultRepo;
        private readonly ITeamInviteRepo _inviteRepo;
        private readonly IJwtHelper _jwtHelper;

        public TeamInviteService(IAccountRepo accountRepo, ITeamRepo teamRepo, ITeamVaultRepo teamVaultRepo, ITeamInviteRepo inviteRepo, IJwtHelper jwtHelper)
        {
            _accountRepo = accountRepo;
            _teamRepo = teamRepo;
            _teamVaultRepo = teamVaultRepo;
            _inviteRepo = inviteRepo;
            _jwtHelper = jwtHelper;
        }

        public async Task<CreateTeamInviteReturn> CreateTeamInvite(CreateTeamInviteDto teamInvite, string shortLivedToken)
        {
            CreateTeamInviteReturn teamInviteReturn = new CreateTeamInviteReturn();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if (string.IsNullOrWhiteSpace(teamInvite.InviteRecipient) || accountId == Guid.Empty || string.IsNullOrWhiteSpace(teamInvite.TeamVaultId))
            {
                teamInviteReturn.Success = false;

                return teamInviteReturn;
            }

            bool hasModel = await _teamRepo.HasTeamVaultAndTeamInvitesOpen(Guid.Parse(teamInvite.TeamVaultId));

            if (!hasModel)
            {
                teamInviteReturn.Success = false;

                return teamInviteReturn;
            }

            AccountDataModel? inviteRecipientAccount = await _accountRepo.FindAccountByEmailAddress(teamInvite.InviteRecipient);

            AccountDataModel? inviteSenderAccount = await _accountRepo.FindSingleRecordViaId<AccountDataModel>(accountId);

            if (inviteRecipientAccount == null || inviteSenderAccount == null )
            {
                teamInviteReturn.Success = false;

                return teamInviteReturn;
            }

            TeamInviteDataModel teamInviteCreated = CreatedTeamInvite(accountId, Guid.Parse(teamInvite.TeamVaultId), inviteSenderAccount.Username, inviteRecipientAccount.Username);

            await _inviteRepo.AddAsync(teamInviteCreated);

            teamInviteReturn.Success = true;
            teamInviteReturn.InviteRecipient = teamInviteCreated.InviteRecipient;
            teamInviteReturn.InviteCreatedAt = teamInviteCreated.InviteCreatedAt;

            return teamInviteReturn;
        }

        public async Task<GetCurrentPendingTeamInvitesReturn> GetCurrentPendingTeamInvites(GetCurrentPendingTeamInvitesDto getCurrentPendingInvites, string shortLivedToken)
        {
            GetCurrentPendingTeamInvitesReturn getPendingInvitesResponse = new GetCurrentPendingTeamInvitesReturn();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if(string.IsNullOrWhiteSpace(getCurrentPendingInvites.TeamVaultId) || accountId == Guid.Empty)
            {
                getPendingInvitesResponse.Success = false;

                return getPendingInvitesResponse;
            }

            throw new NotImplementedException();

        }

        public async Task RejectTeamInvite(string shortLivedToken)
        {
            throw new NotImplementedException();
        }

        public async Task RejectAllTeamInvites(string shortLivedToken)
        {
            throw new NotImplementedException();
        }

        private TeamInviteDataModel CreatedTeamInvite(Guid accountId, Guid teamVaultId, string inviteSentBy, string inviteRecipient)
        {
            TeamInviteDataModel teamInvite = new TeamInviteDataModel
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                TeamVaultId = teamVaultId,
                InviteSentBy = inviteSentBy,
                InviteRecipient = inviteRecipient,
                InviteStatus = InviteStatus.Pending.ToString(),
                InviteCreatedAt = DateTime.Now.ToString(),
            };

            return teamInvite;
        }
    }
}
