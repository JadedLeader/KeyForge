using VaultAPI.DataContext;
using KeyForgedShared.SharedDataModels;
using VaultAPI.Interfaces.RepoInterfaces;
using KeyForgedShared.Generics;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace VaultAPI.Repos
{
    public class VaultRepo : GenericRepository<VaultDataModel>, IVaultRepo
    {

        private readonly VaultDataContext _dataContext;
        public VaultRepo(VaultDataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public override Task<VaultDataModel> AddAsync(VaultDataModel databaseModel)
        {
            return base.AddAsync(databaseModel);
        }

        public override Task<VaultDataModel> UpdateAsync(VaultDataModel databaseModel)
        {
            return base.UpdateAsync(databaseModel);
        }

        public override Task<VaultDataModel> DeleteAsync(VaultDataModel databaseModel)
        {
            return base.DeleteAsync(databaseModel);
        }

        public async Task<VaultDataModel> GetVaultViaVaultId(Guid vaultId)
        {
            VaultDataModel? vault = await _dataContext.Vault.Where(v => v.Id == vaultId).FirstOrDefaultAsync();

            if(vault == null)
            {
                return new VaultDataModel();
            }

            return vault;

        }

        public async Task UpdateVaultName(VaultDataModel vaultDataModel, string newVaultName)
        {
            vaultDataModel.VaultName = newVaultName;

            await UpdateAsync(vaultDataModel);

        }

        public async Task<List<VaultDataModel>> GetVaultsViaAccountId(Guid accountId)
        {
            List<VaultDataModel> vaults = await _dataContext.Vault.Where(vi => vi.AccountId == accountId).ToListAsync();

            return vaults;
        }

        public async Task DeleteAllVaultsForAccount(Guid accountId)
        {

            List<VaultDataModel> vaultsToDelete = await _dataContext.Vault.Where(x => x.AccountId == accountId).ToListAsync();

            _dataContext.RemoveRange(vaultsToDelete);

            Log.Information($"Deleting all vaults for account with ID {accountId}");

            await _dataContext.SaveChangesAsync();

        }


    }
}
