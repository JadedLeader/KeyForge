using KeyForgedShared.DTO_s.TeamMemberDTO_s;
using KeyForgedShared.ReturnTypes.TeamMember;

namespace TeamMembersAPI.Interfaces.Services
{
    public interface ITeamMembersService
    {
        Task<CreateTeamMemberReturn> CreateTeamMember(CreateTeamMemberDto createTeamMember, string shortLivedToken);
        Task GetTeamMembersViaTeamVault();
        Task RemoveTeamMember();
    }
}