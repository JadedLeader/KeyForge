using VaultAPI.DataModel;

namespace VaultAPI.Interfaces.RepoInterfaces
{
    public interface IAuthRepo
    {
        Task<AuthDataModel> AddAsync(AuthDataModel databaseModel);
        Task<AuthDataModel> DeleteAsync(AuthDataModel databaseModel);
        Task<AuthDataModel> FindAuthAccountViaId(Guid accountId);
        Task<AuthDataModel> UpdateAsync(AuthDataModel databaseModel);
        Task UpdateLongLivedKey(Guid accountId, string newLongLivedKey);
        Task UpdateShortLivedKey(Guid accountId, string newShortLivedKey);
    }
}