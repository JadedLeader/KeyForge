using KeyForgedShared.Interfaces;
using KeyForgedShared.Projections.TeamVaultProjections;
using KeyForgedShared.ReturnTypes.TeamVault;
using KeyForgedShared.SharedDataModels;

namespace TeamVaultAPI.Interfaces.Repos
{
    public interface ITeamVaultRepo
    {

        public Task<TeamVaultDataModel> AddAsync(TeamVaultDataModel databaseModel);

        public Task<TeamVaultDataModel> DeleteAsync(TeamVaultDataModel databaseModel);

        public Task<TeamVaultDataModel> UpdateAsync(TeamVaultDataModel databaseModel);

        public Task<List<GetTeamWithNoVault>> GetTeamsWithNoVaults(Guid accountId);

        public Task<bool> HasModel<T>(Guid id) where T : IEntity;

        public Task<T> DeleteRecordViaId<T>(Guid id) where T : IEntity;

        public Task<T?> FindSingleRecordViaId<T>(Guid id) where T : IEntity;

        public Task<TeamVaultDataModel> FindTeamVaultViaTeamId(Guid teamId);


    }
}
