using KeyForgedShared.DTO_s.TeamVaultDTO_s;
using KeyForgedShared.Interfaces;
using KeyForgedShared.Projections.TeamVaultProjections;
using KeyForgedShared.ReturnTypes.TeamVault;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeamVaultAPI.Interfaces.Repos;
using TeamVaultAPI.Interfaces.Services;

namespace TeamVaultAPI.Services
{
    public class TeamVaultService : ITeamVaultService
    {

        private readonly IAccountRepo _accountRepo; 
        
        private readonly ITeamRepo _teamRepo;

        private readonly ITeamVaultRepo _teamVaultRepo;

        private readonly IJwtHelper _jwtHelper;
        public TeamVaultService(IAccountRepo accountRepo, ITeamRepo teamRepo, ITeamVaultRepo teamVaultRepo, IJwtHelper jwtHelper)
        {
            _accountRepo = accountRepo;
            _teamRepo = teamRepo;
            _teamVaultRepo = teamVaultRepo;
            _jwtHelper = jwtHelper;
        }

        public async Task<CreateTeamVaultReturn> CreateTeamVault(CreateTeamVaultDto createTeamVault, string shortLivedToken)
        {
            CreateTeamVaultReturn teamVaultResponse = new CreateTeamVaultReturn();

            if(string.IsNullOrWhiteSpace(createTeamVault.TeamId) || string.IsNullOrWhiteSpace(createTeamVault.TeamVaultName)
                || string.IsNullOrWhiteSpace(createTeamVault.TeamVaultDescription) || string.IsNullOrWhiteSpace(createTeamVault.CurrentStatus))
            {
                teamVaultResponse.Success = false; 

                return teamVaultResponse;
            }

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            bool hasAccount = await _accountRepo.HasModel<AccountDataModel>(accountId);   

            bool hasTeam = await _teamRepo.HasModel<TeamDataModel>(Guid.Parse(createTeamVault.TeamId));

            if(!hasAccount || !hasTeam)
            {
                teamVaultResponse.Success = false;

                return teamVaultResponse;
            }

            TeamVaultDataModel createdTeamVault = MapInputToTeamVault(createTeamVault);

            await _teamVaultRepo.AddAsync(createdTeamVault);

            teamVaultResponse.TeamId = createdTeamVault.TeamId;
            teamVaultResponse.TeamVaultId = createdTeamVault.Id;
            teamVaultResponse.TeamVaultDescription = createdTeamVault.TeamVaultDescription;
            teamVaultResponse.TeamVaultName = createdTeamVault.TeamVaultName;
            teamVaultResponse.CurrentStatus = createdTeamVault.CurrentStatus;
            teamVaultResponse.Success = true;

            return teamVaultResponse;


        }

        public async Task<GetTeamWithNoVaultsReturn> GetTeamsWithNoVaults(string shortLivedToken)
        {
            GetTeamWithNoVaultsReturn teamsWithNoVaultsResponse = new GetTeamWithNoVaultsReturn();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if(accountId == Guid.Empty)
            {
                teamsWithNoVaultsResponse.Success = false;

                return teamsWithNoVaultsResponse;
            }

            List<GetTeamWithNoVault> teamsWithNoVaults = await _teamVaultRepo.GetTeamsWithNoVaults(accountId); 

            teamsWithNoVaultsResponse.TeamsWithNoVaults = teamsWithNoVaults;
            teamsWithNoVaultsResponse.Success = true;

            return teamsWithNoVaultsResponse;
        }

        public async Task<DeleteTeamVaultReturn> DeleteTeamVault(DeleteTeamVaultDto deleteTeamVault, string shortLivedToken)
        {
            DeleteTeamVaultReturn deleteTeamVaultResponse = new DeleteTeamVaultReturn();

            if (string.IsNullOrWhiteSpace(deleteTeamVault.TeamVaultId))
            {
                deleteTeamVaultResponse.Success = false;

                return deleteTeamVaultResponse;
            }

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if (accountId == Guid.Empty)
            {
                deleteTeamVaultResponse.Success = false;

                return deleteTeamVaultResponse;
            }

            bool hasTeamVault = await _teamVaultRepo.HasModel<TeamVaultDataModel>(Guid.Parse(deleteTeamVault.TeamVaultId));

            if (!hasTeamVault)
            {
                deleteTeamVaultResponse.Success = false;

                return deleteTeamVaultResponse;
            }

            TeamVaultDataModel deletedTeamVault = await _teamVaultRepo.DeleteRecordViaId<TeamVaultDataModel>(Guid.Parse(deleteTeamVault.TeamVaultId));

            if (deletedTeamVault == null)
            {
                deleteTeamVaultResponse.Success = false;

                return deleteTeamVaultResponse;
            }

            deleteTeamVaultResponse.TeamVaultId = deleteTeamVault.TeamVaultId;
            deleteTeamVaultResponse.Success = true; 

            return deleteTeamVaultResponse;
        }
        
        public async Task UpdateTeamVault(string shortLivedToken)
        {
            throw new NotImplementedException();
        }

        private TeamVaultDataModel MapInputToTeamVault(CreateTeamVaultDto createTeamVault)
        {
            TeamVaultDataModel teamVault = new TeamVaultDataModel
            {
                TeamId = Guid.Parse(createTeamVault.TeamId),
                Id = Guid.NewGuid(),
                TeamVaultName = createTeamVault.TeamVaultName,
                CurrentStatus = createTeamVault.CurrentStatus,
                CreatedAt = DateTime.Now.ToString(),
                TeamVaultDescription = createTeamVault.TeamVaultDescription,
                LastTeamUpdate = DateTime.Now.ToString(),

            }; 

            return teamVault;


        }

    }
}
