using KeyForgedShared.DTO_s.TeamMemberDTO_s;
using KeyForgedShared.Enums;
using KeyForgedShared.ReturnTypes.TeamMember;
using KeyForgedShared.SharedDataModels;
using KeyForgedShared.ValidationType;
using Microsoft.EntityFrameworkCore.Metadata;
using TeamMembersAPI.Interfaces.DomainService;
using TeamMembersAPI.Interfaces.Services;
using KeyForgedShared.HubProjection.TeamMember;

namespace TeamMembersAPI.Services
{
    public class TeamMembersService : ITeamMembersService
    {

        private readonly ITeamMemberDomainService _teamMemberDomainService;
        private readonly ITeamMemberHubService _teamMemberHubService;

        public TeamMembersService(ITeamMemberDomainService teamMemberDomainService)
        {
            _teamMemberDomainService = teamMemberDomainService;
        }

        public async Task<CreateTeamMemberReturn> CreateTeamMember(CreateTeamMemberDto createTeamMember, string shortLivedToken)
        {
            CreateTeamMemberReturn newTeamMember = new();

            CreateTeamMemberValidationResult teamMemberValidated = await _teamMemberDomainService.ValidateCreateTeamMember(createTeamMember, shortLivedToken);

            if (!teamMemberValidated.IsValidated)
            {
                newTeamMember.Success = false;

                return newTeamMember;
            }


            TeamMemberDataModel teamMember = CreateTeamMemberModel(createTeamMember.TeamVaultId, teamMemberValidated.Email, teamMemberValidated.Username);

            await _teamMemberDomainService.AddTeamMember(teamMember);

            await _teamMemberHubService.NotifyTeamMemberAdded(teamMember.TeamVaultId.ToString(), new TeamMemberAddedProjection { Email = teamMember.Email, Username = teamMemberValidated.Username} );

            newTeamMember.Username = teamMember.Username;
            newTeamMember.Email = teamMember.Email;
            newTeamMember.Success = true;

            return newTeamMember;

        }

        public async Task RemoveTeamMember()
        {
            throw new NotImplementedException();
        }

        public async Task GetTeamMembersViaTeamVault()
        {
            throw new NotImplementedException();
        }

        private TeamMemberDataModel CreateTeamMemberModel(string teamVaultId, string email, string username)
        {
            TeamMemberDataModel newTeamMember = new TeamMemberDataModel
            {
                Id = Guid.NewGuid(),
                TeamVaultId = Guid.Parse(teamVaultId),
                Email = email,
                RoleAccess = RoleAccess.Collaborator.ToString(),
                RoleName = string.Empty,
                Username = username
            };

            return newTeamMember;
        }
    }
}
