using KeyForgedShared.SharedDataModels;
using Serilog;

namespace VaultKeysAPI.Interfaces
{
    public interface IAuthRepo
    {
        public Task<AuthDataModel> AddAsync(AuthDataModel databaseModel);

        public Task UpdateShortLivedKey(Guid accountId, string newShortLivedKey);


        public Task UpdateLongLivedKey(Guid accountId, string newLongLivedKey);


        public Task<AuthDataModel> FindAuthAccountViaId(Guid accountId);


        public Task<AuthDataModel> UpdateAsync(AuthDataModel databaseModel);

    }
}
