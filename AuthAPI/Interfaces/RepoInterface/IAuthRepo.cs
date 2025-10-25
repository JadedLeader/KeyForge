using KeyForgedShared.SharedDataModels;


namespace AuthAPI.Interfaces.RepoInterface
{
    public interface IAuthRepo
    {

        public Task<AccountDataModel> AddAccountToTable(AccountDataModel accountModel);

        public Task<AccountDataModel> RemoveAccountFromTable(AccountDataModel accountModel);

        public Task<AccountDataModel> CheckForExistingAccount(Guid accountId);

        public Task<AuthDataModel> AddAuthToTable(AuthDataModel authDataModel);

        public Task<Guid> RemoveAuthFromTable(AuthDataModel authDataModel);

        public Task<AuthDataModel> CheckForExistingAuth(Guid authId);

        public Task<AuthDataModel> UpdateLongLivedToken(AuthDataModel authDataModel, string refreshedLongLivedToken);

        public Task<AuthDataModel> UpdateShortLivedToken(AuthDataModel authDataModel, string refreshedShortLivedToken);

        public Task<AuthDataModel> RevokeLongLivedToken(AuthDataModel authDataModel);

        public Task<AuthDataModel> UpdateExistingAuthKeys(AuthDataModel authDataModel, string longLivedKey, string shortLivedKey);

        public Task RemoveAccountFromTablesViaId(Guid id);

        public Task<AccountDataModel> CheckForExistingAccountViaUsername(string username);

        public Task<AccountDataModel> RetrieveRoleFromAccount(Guid accountId);

        public Task<AuthDataModel> CheckForExistingAuthViaAccountId(Guid accountId);

    }
}
