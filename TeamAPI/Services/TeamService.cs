using KeyForgedShared.DTO_s.TeamDTO_s;
using KeyForgedShared.Interfaces;
using KeyForgedShared.ReturnTypes.Team;
using KeyForgedShared.SharedDataModels;
using TeamAPI.Interfaces.Repos;
using TeamAPI.Interfaces.Services;
using gRPCIntercommunicationService;
using Grpc.Core;
using TeamAPI.Storage;
using System.Collections.Immutable;

namespace TeamAPI.Services
{
    public class TeamService : Team.TeamBase, ITeamService
    {

        private readonly IAccountRepo _accountRepo;

        private readonly ITeamRepo _teamRepo;

        private readonly IJwtHelper _jwtHelper;

        private readonly IStreamingService _streamingService;

        private readonly StreamingStorage _streamingStorage;
        public TeamService(IAccountRepo accountRepo, ITeamRepo teamRepo, IJwtHelper jwtHelper, IStreamingService streamingService, StreamingStorage streamingStorage)
        {
            _accountRepo = accountRepo;
            _teamRepo = teamRepo;
            _jwtHelper = jwtHelper;
            _streamingService = streamingService;
            _streamingStorage = streamingStorage;
        }

        public async Task<CreateTeamReturn> CreateTeam(CreateTeamDto createTeam, string shortLivedToken)
        {
            CreateTeamReturn createTeamResponse = new CreateTeamReturn();

            if(string.IsNullOrWhiteSpace(createTeam.TeamName) || createTeam.MemberCap == 0)
            {
                createTeamResponse.Success = false;

                return createTeamResponse;
            }

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if(accountId == Guid.Empty)
            {
                createTeamResponse.Success = false;

                return createTeamResponse;
            }

            bool hasAccount = await _accountRepo.hasAccount(accountId);

            if (!hasAccount)
            {
                createTeamResponse.Success = false;

                return createTeamResponse;
            }

            TeamDataModel createdTeam = MapToTeamDataModel(accountId, createTeam.TeamName, createTeam.TeamAcceptingInvites, accountId.ToString(), createTeam.MemberCap);

            await _teamRepo.AddAsync(createdTeam);

            StreamTeamCreationResponse streamTeamResponse = MapTeamToStream(createdTeam);

            _streamingStorage.AddToTeamCreation(streamTeamResponse);

            return MapTeamToTeamReturn(createdTeam);


        }

        public async Task<DeleteTeamReturn> DeleteTeam(DeleteTeamDto deleteTeamRequest, string shortLivedToken)
        {

            DeleteTeamReturn deleteTeamResponse = new DeleteTeamReturn();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));
            
            if(accountId == Guid.Empty ||  string.IsNullOrWhiteSpace(deleteTeamRequest.TeamId))
            {
                deleteTeamResponse.Success = false; 

                return deleteTeamResponse;
            }

            bool hasTeam = await _teamRepo.HasTeam(accountId);

            if (!hasTeam)
            {
                deleteTeamResponse.Success = false;

                return deleteTeamResponse;
            }

            TeamDataModel team = await _teamRepo.DeleteTeamViaId(Guid.Parse(deleteTeamRequest.TeamId));

            if (team == null)
            {
                deleteTeamResponse.Success = false;

                return deleteTeamResponse;
            }

            deleteTeamResponse.Success = true;
            deleteTeamResponse.TeamId = team.Id.ToString(); 
            deleteTeamResponse.TeamName = team.TeamName;

            return deleteTeamResponse;

        }

        public async Task<UpdateTeamReturn> UpdateTeam(UpdateTeamDto updateTeamRequest, string shortLivedToken)
        {

            UpdateTeamReturn updateTeamResponse = new UpdateTeamReturn();

            if(updateTeamRequest.TeamId == Guid.Empty && string.IsNullOrWhiteSpace(updateTeamRequest.NewTeamName) && string.IsNullOrWhiteSpace(updateTeamRequest.TeamAcceptingInvites) && updateTeamRequest.MemberCap == 0)
            {
                updateTeamResponse.Success = false; 

                return updateTeamResponse;
            }

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if(accountId == Guid.Empty)
            {
                updateTeamResponse.Success = false;

                return updateTeamResponse;
            }

            TeamDataModel team = await _teamRepo.GetTeamViaId(updateTeamRequest.TeamId);

            if (!string.IsNullOrWhiteSpace(updateTeamRequest.NewTeamName))
            {
                team.TeamName = updateTeamRequest.NewTeamName;
            }

            if (!string.IsNullOrWhiteSpace(updateTeamRequest.TeamAcceptingInvites))
            {
                team.TeamAcceptingInvites = updateTeamRequest.TeamAcceptingInvites;
            }

            if(updateTeamRequest.MemberCap != 0)
            {
                team.MemberCap = updateTeamRequest.MemberCap;
            }

            await _teamRepo.UpdateAsync(team);

            updateTeamResponse.Success = true; 
            updateTeamResponse.TeamAcceptingInvites = team.TeamAcceptingInvites;
            updateTeamResponse.TeamName = team.TeamName;
            updateTeamResponse.MemberCap = team.MemberCap;

            return updateTeamResponse;

        }

        public async Task<GetTeamsReturn> GetTeamsForAccount(string shortLivedToken)
        {
            GetTeamsReturn returningTeamsResponse = new GetTeamsReturn();

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if (accountId == Guid.Empty)
            {
                returningTeamsResponse.Success = false;

                return returningTeamsResponse;
            }

            List<TeamDataModel> teams = await _teamRepo.GetTeamsFromAccountId(accountId); 

            returningTeamsResponse.Teams = teams;
            returningTeamsResponse.Success = true;

            return returningTeamsResponse;


        }

        private TeamDataModel MapToTeamDataModel(Guid accountId, string teamName, string teamAcceptingInvites, string createdBy, int memberCap )
        {
            TeamDataModel newTeam = new TeamDataModel
            {
                AccountId = accountId,
                Id = Guid.NewGuid(),
                TeamName = teamName,
                TeamAcceptingInvites = teamAcceptingInvites,
                CreatedBy = createdBy,
                CreatedAt = DateTime.Now, 
                MemberCap = memberCap
            };

            return newTeam;
        }

        private CreateTeamReturn MapTeamToTeamReturn(TeamDataModel team)
        {
            CreateTeamReturn teamReturn = new CreateTeamReturn
            {
                TeamId = team.Id.ToString(),
                TeamName = team.TeamName,
                TeamAcceptingInvites = team.TeamAcceptingInvites,
                CreatedBy = team.CreatedBy,
                CreatedAt = team.CreatedAt.ToString(),
                MemberCap = team.MemberCap,
                Success = true


            };

            return teamReturn;


        }

        private StreamTeamCreationResponse MapTeamToStream(TeamDataModel team)
        {

            StreamTeamCreationResponse stream = new StreamTeamCreationResponse
            {
                AccountId = team.AccountId.ToString(),
                TeamAcceptingInvites = team.TeamAcceptingInvites,
                CreatedBy = team.CreatedBy,
                CreatedAt = team.CreatedAt.ToString(),
                MemberCap = team.MemberCap,
                TeamId = team.Id.ToString(),
                TeamName = team.TeamName,
                TeamCreationId = Guid.NewGuid().ToString()
            };

            return stream;

        }

    }
}
