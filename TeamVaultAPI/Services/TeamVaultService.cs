using gRPCIntercommunicationService;
using KeyForgedShared.DTO_s.TeamVaultDTO_s;
using KeyForgedShared.Interfaces;
using KeyForgedShared.Projections.TeamVaultProjections;
using KeyForgedShared.ReturnTypes.TeamVault;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Identity.Client;
using TeamVaultAPI.Interfaces.Repos;
using TeamVaultAPI.Interfaces.Services;
using TeamVaultAPI.Storage;

namespace TeamVaultAPI.Services
{
    public class TeamVaultService : ITeamVaultService
    {

        private readonly IAccountRepo _accountRepo; 
        
        private readonly ITeamRepo _teamRepo;

        private readonly ITeamVaultRepo _teamVaultRepo;

        private readonly IJwtHelper _jwtHelper;

        private readonly StreamingStorage _streamingStorage;

        public TeamVaultService(IAccountRepo accountRepo, ITeamRepo teamRepo, ITeamVaultRepo teamVaultRepo, IJwtHelper jwtHelper, StreamingStorage streamingStorage)
        {
            _accountRepo = accountRepo;
            _teamRepo = teamRepo;
            _teamVaultRepo = teamVaultRepo;
            _jwtHelper = jwtHelper;
            _streamingStorage = streamingStorage;
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

            _streamingStorage.AddToSteamTeamVaultCreations(MapTeamVaultToStream(createdTeamVault));

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

            _streamingStorage.AddToStreamTeamVaultDeletions(new StreamTeamVaultDeletionsResponse { TeamVaultId = deletedTeamVault.Id.ToString() });

            deleteTeamVaultResponse.TeamVaultId = deleteTeamVault.TeamVaultId;
            deleteTeamVaultResponse.Success = true; 

            return deleteTeamVaultResponse;
        }
        
        public async Task<UpdateTeamVaultReturn> UpdateTeamVault(UpdateTeamVaultDto updateTeamVault, string shortLivedToken)
        {
            UpdateTeamVaultReturn updateTeamVaultResponse = new UpdateTeamVaultReturn();

            if(string.IsNullOrWhiteSpace(updateTeamVault.TeamVaultId) && string.IsNullOrWhiteSpace(updateTeamVault.TeamVaultName) && 
                string.IsNullOrWhiteSpace(updateTeamVault.TeamVaultDescription) && string.IsNullOrWhiteSpace(updateTeamVault.CurrentStatus))
            {
                updateTeamVaultResponse.Success = false;

                return updateTeamVaultResponse;
            }

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if(accountId == Guid.Empty)
            {
                updateTeamVaultResponse.Success = false; 

                return updateTeamVaultResponse;
            }

            TeamVaultDataModel? teamVault = await _teamVaultRepo.FindSingleRecordViaId<TeamVaultDataModel>(Guid.Parse(updateTeamVault.TeamVaultId));

            if(teamVault == null)
            {
                updateTeamVaultResponse.Success = false;

                return updateTeamVaultResponse;
            }

            if (!string.IsNullOrWhiteSpace(updateTeamVault.TeamVaultName))
            {
                teamVault.TeamVaultName = updateTeamVault.TeamVaultName;
            }

            if (!string.IsNullOrWhiteSpace(updateTeamVault.TeamVaultDescription))
            {
                teamVault.TeamVaultDescription = updateTeamVault.TeamVaultDescription;
            }

            if (!string.IsNullOrWhiteSpace(updateTeamVault.CurrentStatus))
            {
                teamVault.CurrentStatus = updateTeamVault.CurrentStatus;   
            }

            teamVault.LastTeamUpdate = DateTime.Now.ToString();

            await _teamVaultRepo.UpdateAsync(teamVault);

            _streamingStorage.AddToStreamTeamVaultUpdates(MapTeamVaultToStreamUpdate(teamVault));

            updateTeamVaultResponse.Success = true; 
            updateTeamVaultResponse.CurrentStatus = teamVault.CurrentStatus;
            updateTeamVaultResponse.TeamVaultName = teamVault.TeamVaultName;
            updateTeamVaultResponse.TeamVaultDescription = teamVault.TeamVaultDescription;
           
            return updateTeamVaultResponse;
        }

        public async Task<GetTeamVaultReturn> GetTeamVault(GetTeamVaultDto getTeamVault, string shortLivedToken)
        {
            GetTeamVaultReturn getTeamVaultResponse = new GetTeamVaultReturn();

            if (string.IsNullOrWhiteSpace(getTeamVault.TeamId))
            {
                getTeamVaultResponse.Success = false; 

                return getTeamVaultResponse;
            }

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            TeamVaultDataModel? teamVault = await _teamVaultRepo.FindTeamVaultViaTeamId(Guid.Parse(getTeamVault.TeamId));

            if (teamVault == null)
            {
                getTeamVaultResponse.Success = false;

                return getTeamVaultResponse;
            }

            getTeamVaultResponse.TeamVaultId = teamVault.Id.ToString();
            getTeamVaultResponse.TeamVaultName = teamVault.TeamVaultName; 
            getTeamVaultResponse.TeamVaultDescription = teamVault.TeamVaultDescription;
            getTeamVaultResponse.CurrentStatus = teamVault.CurrentStatus;
            getTeamVaultResponse.Success = true; 

            return getTeamVaultResponse;
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

        private StreamTeamVaultCreationResponse MapTeamVaultToStream(TeamVaultDataModel teamVault)
        {
            StreamTeamVaultCreationResponse newResponse = new StreamTeamVaultCreationResponse
            {
                TeamId = teamVault.TeamId.ToString(),
                CurrentStatus = teamVault.CurrentStatus,
                CreatedAt = teamVault.CreatedAt.ToString(),
                LastTeamUpdate = teamVault.LastTeamUpdate.ToString(),
                TeamVaultDescription = teamVault.TeamVaultDescription,
                TeamVaultId = teamVault.Id.ToString(),
                TeamVaultName = teamVault.TeamVaultName,
                TeamVaultCreationId = Guid.NewGuid().ToString(),

            };

            return newResponse;
        }

        private StreamTeamVaultUpdatesResponse MapTeamVaultToStreamUpdate(TeamVaultDataModel teamVault)
        {
            StreamTeamVaultUpdatesResponse newUpdate = new StreamTeamVaultUpdatesResponse
            {
                CurrentStatus = teamVault.CurrentStatus,
                LastTeamUpdate = teamVault.LastTeamUpdate,
                TeamVaultDescription = teamVault.TeamVaultDescription,
                TeamVaultId = teamVault.Id.ToString(),
                TeamVaultName = teamVault.TeamVaultName,
            }; 

            return newUpdate;
        }

    }
}
