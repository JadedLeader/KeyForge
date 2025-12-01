using gRPCIntercommunicationService;
using System.Collections.Immutable;

namespace TeamInviteAPI.StreamingStorage
{
    public class TeamInviteStreamingStorage
    {

        private List<StreamTeamInviteCreationsResponse> _teamInviteCreations = new();

        private List<StreamTeamInviteDeletionResponse> _teamInviteDeletions = new(); 

        public void AddToTeamInviteCreations(StreamTeamInviteCreationsResponse teamInviteCreations)
        {
            _teamInviteCreations.Add(teamInviteCreations);
        }

        public void AddToTeamInviteDeletions(StreamTeamInviteDeletionResponse teamInviteDeletion)
        {
            _teamInviteDeletions.Add(teamInviteDeletion);
        }

        public ImmutableList<StreamTeamInviteCreationsResponse> ReturnTeamInviteCreations()
        {
            return _teamInviteCreations.ToImmutableList();
        }

        public ImmutableList<StreamTeamInviteDeletionResponse> ReturnTeamInviteDeletions()
        {
            return _teamInviteDeletions.ToImmutableList();
        }

        public void ClearTeamInviteCreation()
        {
            _teamInviteCreations.Clear();
        }

        public void ClearTeamInviteDeletion()
        {
            _teamInviteDeletions.Clear();
        }

    }
}
