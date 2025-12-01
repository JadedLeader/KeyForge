using KeyForgedShared.SharedDataModels;

namespace TeamInviteAPI.Interfaces.Services
{
    public interface ISignalRInviteService
    {
        Task PushInviteToHub(TeamInviteDataModel inviteDataModel);
    }
}