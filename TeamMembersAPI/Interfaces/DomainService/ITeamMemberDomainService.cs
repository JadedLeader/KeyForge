using KeyForgedShared.DTO_s.TeamMemberDTO_s;
using KeyForgedShared.SharedDataModels;
using KeyForgedShared.ValidationType;

namespace TeamMembersAPI.Interfaces.DomainService
{
    public interface ITeamMemberDomainService
    {
        Task<CreateTeamMemberValidationResult> ValidateCreateTeamMember(CreateTeamMemberDto createTeamMember, string shortLivedToken);

        Task<TeamMemberDataModel> AddTeamMember(TeamMemberDataModel teamMember);
    }
}