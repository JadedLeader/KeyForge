using gRPCIntercommunicationService;
using Serilog;
using System.Collections.Immutable;

namespace TeamAPI.Storage
{
    public class StreamingStorage 
    {

        private List<StreamTeamCreationResponse> _teamCreations = new();

        private List<StreamTeamDeletionResponse> _teamDeletions = new();

        private List<StreamTeamUpdateResponse> _teamUpdates = new();


        public void AddToTeamCreation(StreamTeamCreationResponse team)
        {

            Log.Information($"Team added to list");
            _teamCreations.Add(team);
        }

        public void AddToTeamDeletions(StreamTeamDeletionResponse teamDeletion)
        {
            _teamDeletions.Add(teamDeletion);
        }

        public void AddToTeamUpdates(StreamTeamUpdateResponse teamUpdates)
        {
            _teamUpdates.Add(teamUpdates);
        }


        public ImmutableList<StreamTeamCreationResponse> ReturnTeamCreations()
        {
            return _teamCreations.ToImmutableList();
        }

        public ImmutableList<StreamTeamDeletionResponse> ReturnTeamDeletions()
        {
            return _teamDeletions.ToImmutableList();
        }

        public ImmutableList<StreamTeamUpdateResponse> ReturnTeamUpdates()
        {
            return _teamUpdates.ToImmutableList();
        }

        public void ClearTeamCreations()
        {
            _teamCreations.Clear();
        }

        public void ClearTeamDeletions()
        {
            _teamDeletions.Clear();
        }

        public void ClearTeamUpdates()
        {
            _teamUpdates.Clear();
        }


    }
}
