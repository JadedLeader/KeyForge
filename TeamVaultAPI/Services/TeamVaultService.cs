using KeyForgedShared.DTO_s.TeamVaultDTO_s;
using KeyForgedShared.Interfaces;
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

            bool hasTeam = await _teamRepo.HasModel<TeamDataModel>(accountId);

            if(!hasAccount || !hasTeam)
            {
                teamVaultResponse.Success = false;

                return teamVaultResponse;
            }

            TeamVaultDataModel createdTeamVault = MapInputToTeamVault(createTeamVault);

            await _teamVaultRepo.AddAsync(createdTeamVault);

            teamVaultResponse.TeamId = createdTeamVault.TeamId;
            teamVaultResponse.TeamVaultId = createdTeamVault.TeamVaultId;
            teamVaultResponse.TeamVaultDescription = createdTeamVault.TeamVaultDescription;
            teamVaultResponse.TeamVaultName = createdTeamVault.TeamVaultName;
            teamVaultResponse.CurrentStatus = createdTeamVault.CurrentStatus;
            teamVaultResponse.Success = true;

            return teamVaultResponse;


        }

        public async Task DeleteTeamVault(string shortLivedToken)
        {
            throw new NotImplementedException();
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
                TeamVaultId = Guid.NewGuid(),
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
