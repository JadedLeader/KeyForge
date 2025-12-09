using Google.Protobuf.WellKnownTypes;
using gRPCIntercommunicationService;
using KeyForgedShared.DTO_s.TeamInviteDTO_s;
using KeyForgedShared.Enums;
using KeyForgedShared.Interfaces;
using KeyForgedShared.Projections.TeamInviteProjections;
using KeyForgedShared.ReturnTypes.TeamInvite;
using KeyForgedShared.SharedDataModels;
using KeyForgedShared.ValidationType;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using TeamInviteAPI.DomainServices;
using TeamInviteAPI.Interfaces.DomainServices;
using TeamInviteAPI.Interfaces.Repos;
using TeamInviteAPI.Interfaces.Services;
using TeamInviteAPI.StreamingStorage;


namespace TeamInviteAPI.Services
{
    public class TeamInviteService : ITeamInviteService
    {

        private readonly ITeamInviteDomainService _teamInviteDomain;
        private readonly IJwtHelper _jwtHelper;
        private readonly TeamInviteStreamingStorage _streamingStorage;
        private readonly ISignalRInviteService _signalRInviteService;

        public TeamInviteService(ITeamInviteDomainService teamInviteDomain, IJwtHelper jwtHelper, TeamInviteStreamingStorage streamingStorage, ISignalRInviteService signalRInviteService)
        {
            _teamInviteDomain = teamInviteDomain;
            _jwtHelper = jwtHelper;
            _streamingStorage = streamingStorage;
            _signalRInviteService = signalRInviteService;
        }

        public async Task<CreateTeamInviteReturn> CreateTeamInvite(CreateTeamInviteDto teamInvite, string shortLivedToken)
        {
            CreateTeamInviteReturn teamInviteReturn = new CreateTeamInviteReturn();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            CreateInviteValidationResult validationResult = await _teamInviteDomain.ValidateInviteCreation(teamInvite, accountId);

            if(!validationResult.ValidationSucess)
            {
                teamInviteReturn.Success = false;

                return teamInviteReturn;
            }

            TeamInviteDataModel teamInviteCreated = CreatedTeamInvite(accountId, Guid.Parse(teamInvite.TeamVaultId), validationResult.senderAccountEmail, teamInvite.InviteRecipient);

            _streamingStorage.AddToTeamInviteCreations(MapTeamInviteToStreamResponse(teamInviteCreated));

            await _teamInviteDomain.CreateTeamInvite(teamInviteCreated);

            await _signalRInviteService.PushInviteToHub(teamInviteCreated);

            teamInviteReturn.Success = true;
            teamInviteReturn.InviteRecipient = teamInviteCreated.InviteRecipient;
            teamInviteReturn.InviteCreatedAt = teamInviteCreated.InviteCreatedAt;

            return teamInviteReturn;
        }

        public async Task<GetCurrentPendingTeamInvitesReturn> GetCurrentPendingTeamInvites(GetCurrentPendingTeamInvitesDto getCurrentPendingInvites, string shortLivedToken)
        {
            GetCurrentPendingTeamInvitesReturn getPendingInvitesResponse = new();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            GetPendingInvitesValidationResult pendingInvitesValidation = await _teamInviteDomain.ValidatePendingInvites(getCurrentPendingInvites, accountId);

            if (!pendingInvitesValidation.isValidated)
            {
                getPendingInvitesResponse.Success = false; 

                return getPendingInvitesResponse;
            }

            getPendingInvitesResponse.PendingTeamInvites = pendingInvitesValidation.PendingTeamInvites;
            getPendingInvitesResponse.Success = true; 

            return getPendingInvitesResponse;

        }

        public async Task<GetAllPendingInvitesForAccountReturn> GetAllPendingInvitesForAccount(GetAllPendingInvitesForAccountDto getPendingInvites, string shortLivedToken)
        {
            GetAllPendingInvitesForAccountReturn allPendingInvites = new();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            GetPendingInvitesValidationResult isValidated = await _teamInviteDomain.ValidateGetAllPendingInvitesForAccount(getPendingInvites, accountId);

            if (!isValidated.isValidated)
            {
                allPendingInvites.Success = false;

                return allPendingInvites;
            }

            allPendingInvites.Success = true;
            allPendingInvites.PendingTeamInvites = isValidated.PendingTeamInvites;

            return allPendingInvites;

        }

        public async Task<RejectTeamInviteReturn> RejectTeamInvite(RejectTeamInviteDto rejectTeamInvite, string shortLivedToken)
        {
            RejectTeamInviteReturn rejectTeamInviteResponse = new();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            bool rejectTeamInviteValidation = await _teamInviteDomain.ValidateTeamInviteRejection(rejectTeamInvite, accountId);

            if (!rejectTeamInviteValidation)
            {
                rejectTeamInviteResponse.Success = false;

                return rejectTeamInviteResponse;
            }

            _streamingStorage.AddToTeamInviteDeletions(new StreamTeamInviteDeletionResponse { TeamInviteId = rejectTeamInvite.TeamInviteId });

            TeamInviteDataModel deletedTeamInvite = await _teamInviteDomain.DeleteTeamInvite(Guid.Parse(rejectTeamInvite.TeamInviteId));

            rejectTeamInviteResponse.TeamInviteId = deletedTeamInvite.Id.ToString();
            rejectTeamInviteResponse.Success = true;

            return rejectTeamInviteResponse;

        }

        public async Task RejectAllTeamInvites(string shortLivedToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateTeamInviteReturn> UpdateTeamInvite(UpdateTeamInviteDto updateTeamInvite, string shortLivedToken)
        {
            UpdateTeamInviteReturn updateTeamInviteResponse = new();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            UpdateTeamInviteValidationResult updateTeamInviteResult = await _teamInviteDomain.ValidateUpdateTeamInvite(updateTeamInvite);

            if (!updateTeamInviteResult.IsValidated)
            {
                updateTeamInviteResponse.Success = false;

                return updateTeamInviteResponse;
            }

            updateTeamInviteResult.TeamInvite.InviteStatus = updateTeamInvite.InviteStatus;

            TeamInviteDataModel updatedTeamInvite = await _teamInviteDomain.UpdateTeamInvite(updateTeamInviteResult.TeamInvite);

            _streamingStorage.AddToTeamInviteUpdates(MapTeamInviteToUpdateResponse(updatedTeamInvite));

            updateTeamInviteResponse.Success = true;
            updateTeamInviteResponse.InviteStatus = updatedTeamInvite.InviteStatus;

            return updateTeamInviteResponse;


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

        private StreamTeamInviteCreationsResponse MapTeamInviteToStreamResponse(TeamInviteDataModel teamInvite)
        {
            StreamTeamInviteCreationsResponse teamInviteCreation = new StreamTeamInviteCreationsResponse
            {
                TeamInviteId = teamInvite.Id.ToString(),
                TeamVaultId = teamInvite.TeamVaultId.ToString(),
                AccountId = teamInvite.AccountId.ToString(),
                InviteSentBy = teamInvite.InviteSentBy,
                InviteStatus = teamInvite.InviteStatus,
                InviteCreatedAt = teamInvite.InviteCreatedAt,
                InviteRecipient = teamInvite.InviteRecipient,
                TeamInviteCreationId = Guid.NewGuid().ToString(),
            };

            return teamInviteCreation;
        }

        private StreamTeamInviteUpdateResponse MapTeamInviteToUpdateResponse(TeamInviteDataModel teamInvite)
        {
            StreamTeamInviteUpdateResponse teamUpdates = new StreamTeamInviteUpdateResponse
            {
                InviteStatus = teamInvite.InviteStatus,
                TeamInviteId = teamInvite.Id.ToString()
            };

            return teamUpdates;
        }

    
    }
}
