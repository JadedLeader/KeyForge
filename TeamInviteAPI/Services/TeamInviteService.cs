using KeyForgedShared.DTO_s.TeamInviteDTO_s;
using KeyForgedShared.Interfaces;
using KeyForgedShared.ReturnTypes.TeamInvite;
using TeamInviteAPI.Interfaces.Repos;

namespace TeamInviteAPI.Services
{
    public class TeamInviteService
    {

        private readonly IAccountRepo _accountRepo; 
        private readonly ITeamRepo _teamRepo;
        private readonly ITeamVaultRepo _teamVaultRepo;
        private readonly ITeamInviteRepo _inviteRepo;
        private readonly IJwtHelper _jwtHelper;
  
        public TeamInviteService(IAccountRepo accountRepo, ITeamRepo teamRepo, ITeamVaultRepo teamVaultRepo, ITeamInviteRepo inviteRepo, IJwtHelper jwtHelper)
        {
            _accountRepo = accountRepo;
            _teamRepo = teamRepo;
            _teamVaultRepo = teamVaultRepo;
            _inviteRepo = inviteRepo;
            _jwtHelper = jwtHelper;
        }

        public async Task<CreateTeamInviteReturn> CreateTeamInvite(CreateTeamInviteDto teamInvite, string shortLivedToken)
        {
            throw new NotImplementedException();
        }

        public async Task RejectTeamInvite(string shortLivedToken)
        {
            throw new NotImplementedException();
        }

        public async Task RejectAllTeamInvites(string shortLivedToken)
        {
            throw new NotImplementedException();
        }

    }
}
