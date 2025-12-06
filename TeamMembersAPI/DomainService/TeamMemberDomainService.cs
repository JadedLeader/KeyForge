using KeyForgedShared.DTO_s.TeamMemberDTO_s;
using KeyForgedShared.Interfaces;
using KeyForgedShared.SharedDataModels;
using KeyForgedShared.ValidationType;
using System.Threading.Tasks;
using TeamMembersAPI.Interfaces.DomainService;
using TeamMembersAPI.Interfaces.Repo;

namespace TeamMembersAPI.DomainService
{
    public class TeamMemberDomainService : ITeamMemberDomainService
    {

        private readonly ITeamVaultRepo _teamVaultRepo;

        private readonly ITeamInviteRepo _teamInviteRepo;

        private readonly ITeamRepo _teamRepo;

        private readonly IAccountRepo _accountRepo;

        private readonly ITeamMemberRepo _teamMemberRepo;

        private readonly IJwtHelper _jwtHelper;

        public TeamMemberDomainService(ITeamVaultRepo teamVaultRepo, ITeamInviteRepo teamInviteRepo, ITeamRepo teamRepo, IJwtHelper jwtHelper)
        {
            _teamVaultRepo = teamVaultRepo;
            _teamInviteRepo = teamInviteRepo;
            _teamRepo = teamRepo;
            _jwtHelper = jwtHelper;
        }

        private bool ValidateCreateTeamMemberInput(CreateTeamMemberDto createTeamMember)
        {
            if (string.IsNullOrWhiteSpace(createTeamMember.TeamVaultId) || string.IsNullOrWhiteSpace(createTeamMember.TeamInviteId))
            {
                return false;
            }

            return true;
        }

        private async Task<AccountDataModel> ValidateHasAccount(string shortLivedToken)
        {

            Guid accountId = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            AccountDataModel? account = await _accountRepo.FindSingleRecordViaId<AccountDataModel>(accountId);

            if (account == null)
            {
                return null;
            }

            return account;
        }

        private async Task<bool> ValidateHasTeamInvite(Guid teamInviteId)
        {
            bool hasTeamInvite = await _teamInviteRepo.HasModel<TeamInviteDataModel>(teamInviteId);

            if (!hasTeamInvite)
            {
                return false;
            }

            return true;
        }

        public async Task<CreateTeamMemberValidationResult> ValidateCreateTeamMember(CreateTeamMemberDto createTeamMember, string shortLivedToken)
        {

            CreateTeamMemberValidationResult validationResult = new();

            if (!ValidateCreateTeamMemberInput(createTeamMember))
            {
                validationResult.IsValidated = false;

                return validationResult;
            }

            AccountDataModel? account = await ValidateHasAccount(shortLivedToken);

            if (account == null)
            {
                validationResult.IsValidated = false;

                return validationResult;
            }

            if (!await ValidateHasTeamInvite(Guid.Parse(createTeamMember.TeamInviteId)))
            {
                validationResult.IsValidated = false;

                return validationResult;
            }

            validationResult.IsValidated = true;
            validationResult.Username = account.Username;
            validationResult.Email = account.Email;

            return validationResult;






        }

        public async Task<TeamMemberDataModel> AddTeamMember(TeamMemberDataModel teamMember)
        {
            return await _teamMemberRepo.AddAsync(teamMember);
        }


    }
}
