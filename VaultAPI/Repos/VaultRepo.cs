﻿using VaultAPI.DataContext;
using KeyForgedShared.SharedDataModels;
using VaultAPI.Interfaces.RepoInterfaces;
using KeyForgedShared.Generics;
using Microsoft.EntityFrameworkCore;

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
            VaultDataModel? vault = await _dataContext.Vault.Where(v => v.VaultId == vaultId).FirstOrDefaultAsync();

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



    }
}
