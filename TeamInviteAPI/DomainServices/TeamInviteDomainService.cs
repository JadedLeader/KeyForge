using gRPCIntercommunicationService;
using KeyForgedShared.DTO_s.TeamInviteDTO_s;
using KeyForgedShared.Projections.TeamInviteProjections;
using KeyForgedShared.SharedDataModels;
using KeyForgedShared.ValidationType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using TeamInviteAPI.Interfaces.DomainServices;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.DomainServices
{
    public class TeamInviteDomainService : ITeamInviteDomainService
    {


        private readonly ITeamInviteRepo _teamInviteRepo;

        private readonly IAccountRepo _accountRepo;

        private readonly ITeamRepo _teamRepo;
        public TeamInviteDomainService(ITeamInviteRepo teamInviteRepo, IAccountRepo accountRepo, ITeamRepo teamRepo)
        {
            _teamInviteRepo = teamInviteRepo;
            _accountRepo = accountRepo;
            _teamRepo = teamRepo;
        }

        private async Task<bool> ValidateAccountAlreadyInvited(string recipientEmail, Guid teamVaultId)
        {

            TeamInviteDataModel? existingInviteFromTeam = await _teamInviteRepo.GetTeamInviteViateamVautAndRecipient(recipientEmail, teamVaultId);

            if (existingInviteFromTeam == null)
            {
                return false;
            }

            return true;

        }

        private async Task<bool> ValidateHasAccountWithEmailAddress(string email)
        {
            AccountDataModel? inviteRecipientAccount = await _accountRepo.FindAccountByEmailAddress(email);

            if (inviteRecipientAccount == null)
            {
                return false;
            }

            return true;
        }

        private async Task<string> ValidateSenderAccount(Guid accountId)
        {
            AccountDataModel? inviteSenderAccount = await _accountRepo.FindSingleRecordViaId<AccountDataModel>(accountId);

            if (inviteSenderAccount == null)
            {
                return null;
            }

            return inviteSenderAccount.Email;
        }

        private async Task<bool> ValidateTeamVaultAndTeamInvitesOpen(Guid teamVaultId)
        {
            bool hasModel = await _teamRepo.HasTeamVaultAndTeamInvitesOpen(teamVaultId);

            if (!hasModel)
            {
                return false;
            }

            return true;
        }

        public async Task<TeamInviteDataModel> CreateTeamInvite(TeamInviteDataModel teamInvite)
        {
            return await _teamInviteRepo.AddAsync(teamInvite);
        }

        private bool ValidateUserInput(CreateTeamInviteDto teamInvite)
        {
            if (string.IsNullOrWhiteSpace(teamInvite.InviteRecipient) || string.IsNullOrWhiteSpace(teamInvite.TeamVaultId))
            {

                return false;

            }

            return true;
        }

        public async Task<CreateInviteValidationResult> ValidateInviteCreation(CreateTeamInviteDto teamInvite, Guid senderAccountId)
        {

            CreateInviteValidationResult validationResult = new();

            if (!ValidateUserInput(teamInvite))
            {
                validationResult.ValidationSucess = false;

                return validationResult;
            }

            if (await ValidateAccountAlreadyInvited(teamInvite.InviteRecipient, Guid.Parse(teamInvite.TeamVaultId)))
            {
                validationResult.ValidationSucess = false;

                return validationResult;
            }

            if (!await ValidateHasAccountWithEmailAddress(teamInvite.InviteRecipient))
            {
                validationResult.ValidationSucess = false;

                return validationResult;
            }

            string senderAccountEmail = await ValidateSenderAccount(senderAccountId);

            if (senderAccountEmail == null)
            {
                validationResult.ValidationSucess = false;

                return validationResult;
            }

            if (!await ValidateTeamVaultAndTeamInvitesOpen(Guid.Parse(teamInvite.TeamVaultId)))
            {
                validationResult.ValidationSucess = false;

                return validationResult;
            }

            validationResult.ValidationSucess = true;
            validationResult.senderAccountEmail = senderAccountEmail;

            return validationResult;

        }

        private bool ValidateGetPendingInvitesInput(GetCurrentPendingTeamInvitesDto getPendingInvites, Guid accountId)
        {
            if (string.IsNullOrWhiteSpace(getPendingInvites.TeamVaultId) || accountId == Guid.Empty)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateOwnerOfTeam(Guid accountId)
        {
            bool isOwnerOfTeamVault = await _teamRepo.OwnerOfTeam(accountId);

            if (!isOwnerOfTeamVault)
            {
                return false;
            }

            return true;
        }

        private async Task<List<PendingTeamInvitesProjection>> GetPendingTeamInvites(Guid teamVaultId)
        {
            List<PendingTeamInvitesProjection> pendingInvites = await _teamInviteRepo.GetTeamVaultPendingInvites(teamVaultId);

            return pendingInvites;
        }

        public async Task<GetPendingInvitesValidationResult> ValidatePendingInvites(GetCurrentPendingTeamInvitesDto pendingTeamInvites, Guid accountId)
        {

            GetPendingInvitesValidationResult pendingInvitesValidation = new();

            if(!ValidateGetPendingInvitesInput(pendingTeamInvites, accountId)){

                pendingInvitesValidation.isValidated = false;

                return pendingInvitesValidation;
            }

            if(!await ValidateOwnerOfTeam(accountId))
            {
                pendingInvitesValidation.isValidated = false;

                return pendingInvitesValidation;
            }

            pendingInvitesValidation.isValidated = true; 
            pendingInvitesValidation.PendingTeamInvites = await GetPendingTeamInvites(accountId);

            return pendingInvitesValidation;


        }

        private bool ValidateRejectTeamInviteInput(RejectTeamInviteDto rejectTeamInvite, Guid accountId)
        {
            if (string.IsNullOrWhiteSpace(rejectTeamInvite.TeamInviteId) || accountId == Guid.Empty)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateTeamInviteRejection(RejectTeamInviteDto rejectTeamInvite, Guid accountId)
        {
            if (!ValidateRejectTeamInviteInput(rejectTeamInvite, accountId))
            {
                return false;
            }
                
            if(!await _teamInviteRepo.HasModel<TeamInviteDataModel>(Guid.Parse(rejectTeamInvite.TeamInviteId)))
            {
                return false;
            }

            return true;
        }

        public async Task<TeamInviteDataModel> DeleteTeamInvite(Guid teamInviteId)
        {
            TeamInviteDataModel? deletedVault = await _teamInviteRepo.DeleteRecordViaId<TeamInviteDataModel>(teamInviteId);

            if (deletedVault == null)
            {
                return null;
            }

            return deletedVault;

        }

        private bool ValidateUpdateTeamInviteInput(UpdateTeamInviteDto updateTeamInvite)
        {
            if(string.IsNullOrWhiteSpace(updateTeamInvite.TeamInviteId) || string.IsNullOrWhiteSpace(updateTeamInvite.InviteStatus))
            {
                return false;
            }

            return true;
        }

        private async Task<TeamInviteDataModel> ValidateTeamInviteExists(UpdateTeamInviteDto updateTeamInvite)
        {
            TeamInviteDataModel? teamInviteExists = await _teamInviteRepo.FindSingleRecordViaId<TeamInviteDataModel>(Guid.Parse(updateTeamInvite.TeamInviteId));

            if(teamInviteExists == null)
            {
                return null;
            }

            return teamInviteExists;
        }

        public async Task<UpdateTeamInviteValidationResult> ValidateUpdateTeamInvite(UpdateTeamInviteDto updateTeamInvite)
        {

            UpdateTeamInviteValidationResult teamInviteValidation = new();

            if (!ValidateUpdateTeamInviteInput(updateTeamInvite))
            {
                teamInviteValidation.IsValidated = false;

                return teamInviteValidation;
            }

            TeamInviteDataModel? inviteExists = await ValidateTeamInviteExists(updateTeamInvite);

            if(inviteExists == null)
            {
                teamInviteValidation.IsValidated = false;

                return teamInviteValidation;
            }

            teamInviteValidation.IsValidated = true;
            teamInviteValidation.TeamInvite = inviteExists;

            return teamInviteValidation;

        }

        public async Task<TeamInviteDataModel> UpdateTeamInvite(TeamInviteDataModel updateTeamInvite)
        {
            TeamInviteDataModel teamInvite = await _teamInviteRepo.UpdateAsync(updateTeamInvite);

            return teamInvite;
        }

        private bool ValidateGetAllPendingInvitesInput(GetAllPendingInvitesForAccountDto allPendingInvites, Guid accountId)
        {
            if(string.IsNullOrWhiteSpace(allPendingInvites.Email) || accountId == Guid.Empty)
            {
                return false;
            }

            return true;
        }

        

        public async Task<GetPendingInvitesValidationResult> ValidateGetAllPendingInvitesForAccount(GetAllPendingInvitesForAccountDto pendingInvites, Guid accountId)
        {

            GetPendingInvitesValidationResult pendingInvitesValidation = new();

            if (!ValidateGetAllPendingInvitesInput(pendingInvites, accountId))
            {
                pendingInvitesValidation.isValidated = false;

                return pendingInvitesValidation;
            }

            List<PendingTeamInvitesProjection> allPendingInvites = await _teamInviteRepo.GetPendingInvitesForAccount(pendingInvites.Email);

            pendingInvitesValidation.isValidated = true;
            pendingInvitesValidation.PendingTeamInvites = allPendingInvites;

            return pendingInvitesValidation;
        }

    }
}
