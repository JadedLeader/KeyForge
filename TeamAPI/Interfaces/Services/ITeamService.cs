using KeyForgedShared.DTO_s.TeamDTO_s;
using KeyForgedShared.ReturnTypes.Team;

namespace TeamAPI.Interfaces.Services
{
    public interface ITeamService
    {

        public Task<CreateTeamReturn> CreateTeam(CreateTeamDto createTeam, string shortLivedToken);

        public Task<GetTeamsReturn> GetTeamsForAccount(string shortLivedToken);

        public Task<DeleteTeamReturn> DeleteTeam(DeleteTeamDto deleteTeamRequest, string shortLivedToken);

    }
}
