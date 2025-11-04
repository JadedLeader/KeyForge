using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VaultKeysAPI.DataContext;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.Repos
{
    public class AuthRepo : GenericRepository<AuthDataModel>, IAuthRepo
    {
        private readonly VaultKeysDataContext _vaultKeysDataContext;

        public AuthRepo(VaultKeysDataContext vaultKeysDataContext) : base(vaultKeysDataContext)
        {
            _vaultKeysDataContext = vaultKeysDataContext;
        }

        public override Task<AuthDataModel> DeleteAsync(AuthDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }
        public override Task<AuthDataModel> AddAsync(AuthDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public async Task UpdateShortLivedKey(Guid accountId, string newShortLivedKey)
        {
            AuthDataModel? authAccount = await FindAuthAccountViaId(accountId);

            if (authAccount == null)
            {
                Log.Warning($"{this.GetType().Namespace} Cannot find auth account relationg to this ID");

                return;
            }

            authAccount.ShortLivedKey = newShortLivedKey;

            await UpdateAsync(authAccount);

        }

        public async Task UpdateLongLivedKey(Guid accountId, string newLongLivedKey)
        {
            AuthDataModel? authAccount = await FindAuthAccountViaId(accountId);

            if (authAccount.AccountId == Guid.Empty)
            {
                Log.Warning($"No auth record can be found");

                return;
            }

            authAccount.LongLivedKey = newLongLivedKey;

            await UpdateAsync(authAccount);
        }

        public async Task<AuthDataModel> FindAuthAccountViaId(Guid accountId)
        {
            AuthDataModel? authAccount = await _vaultKeysDataContext.Auth.Where(ac => ac.AccountId == accountId).FirstOrDefaultAsync();

            if (authAccount == null)
            {
                Log.Warning($"{this.GetType().Namespace} Cannot find auth account relationg to this ID");

                return new AuthDataModel();
            }

            return authAccount;
        }
    }
}
