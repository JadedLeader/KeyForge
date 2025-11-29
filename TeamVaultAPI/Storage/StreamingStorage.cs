using gRPCIntercommunicationService;
using System.Collections.Immutable;

namespace TeamVaultAPI.Storage
{
    public class StreamingStorage
    {

        private List<StreamTeamVaultCreationResponse> _steamTeamVaultCreations = new();

        private List<StreamTeamVaultDeletionsResponse> _streamTeamVaultDeletions = new();   

        private List<StreamTeamVaultUpdatesResponse> _streamTeamVaultUpdates = new();


        public void AddToSteamTeamVaultCreations(StreamTeamVaultCreationResponse streamTeamVaultCreation)
        {
            _steamTeamVaultCreations.Add(streamTeamVaultCreation);
        }

        public void AddToStreamTeamVaultDeletions(StreamTeamVaultDeletionsResponse streamTeamVaultDeletions)
        {
            _streamTeamVaultDeletions.Add(streamTeamVaultDeletions);
        }

        public void AddToStreamTeamVaultUpdates (StreamTeamVaultUpdatesResponse streamTeamVaultUpdates)
        {
            _streamTeamVaultUpdates.Add(streamTeamVaultUpdates);
        }

        public ImmutableList<StreamTeamVaultCreationResponse> ReturnSteamTeamVaultCreations()
        {
            return _steamTeamVaultCreations.ToImmutableList();
        }

        public ImmutableList<StreamTeamVaultDeletionsResponse> ReturnStreamTeamVaultDeletions()
        {
            return _streamTeamVaultDeletions.ToImmutableList();
        }

        public ImmutableList<StreamTeamVaultUpdatesResponse> ReturnSteamTeamVaultUpdates()
        {
            return _streamTeamVaultUpdates.ToImmutableList();
        }

        public void ClearStreamTeamVaultCreations()
        {
            _steamTeamVaultCreations.Clear();
        }

        public void ClearStreamTeamVaultDeletions()
        {
            _streamTeamVaultDeletions.Clear();
        }

        public void ClearStreamTeamVaultUpdates()
        {
            _streamTeamVaultUpdates.Clear();
        }

    }
}
