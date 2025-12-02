using KeyForgedShared.Interfaces;
using KeyForgedShared.SharedDataModels;

namespace TeamMembersAPI.Interfaces.Repo
{
    public interface ITeamMemberRepo
    {
        Task<TeamMemberDataModel> AddAsync(TeamMemberDataModel databaseModel);
        Task<TeamMemberDataModel> DeleteAsync(TeamMemberDataModel databaseModel);
        Task<T> DeleteRecordViaId<T>(Guid id) where T : IEntity;
        Task<List<T>> FindAllRecordsViaId<T>(Guid id) where T : IEntity;
        Task<T?> FindSingleRecordViaId<T>(Guid id) where T : IEntity;
        Task<bool> HasModel<T>(Guid id) where T : IEntity;
        Task<TeamMemberDataModel> UpdateAsync(TeamMemberDataModel databaseModel);
    }
}