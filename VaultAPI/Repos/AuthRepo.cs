using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Serilog;
using System.Diagnostics;
using VaultAPI.DataContext;
using VaultAPI.Interfaces.RepoInterfaces;
using KeyForgedShared.Generics;
using KeyForgedShared.SharedDataModels;

namespace VaultAPI.Repos
{
    public class AuthRepo : GenericRepository<AuthDataModel>, IAuthRepo
    {

        private readonly VaultDataContext _dataContext;
        public AuthRepo(VaultDataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public override Task<AuthDataModel> AddAsync(AuthDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<AuthDataModel> DeleteAsync(AuthDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public override Task<AuthDataModel> UpdateAsync(AuthDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
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

            if (authAccount == null)
            {
                Log.Warning($"No auth record can be found");

                return;
            }

            authAccount.LongLivedKey = newLongLivedKey;

            await UpdateAsync(authAccount);
        }

        public async Task<AuthDataModel> FindAuthAccountViaId(Guid accountId)
        {
            AuthDataModel? authAccount = await _dataContext.Auth.Where(ac => ac.AccountId == accountId).FirstOrDefaultAsync();

            if (authAccount == null)
            {
                Log.Warning($"{this.GetType().Namespace} Cannot find auth account relationg to this ID");

                return null;
            }

            return authAccount;
        }


    }
}
