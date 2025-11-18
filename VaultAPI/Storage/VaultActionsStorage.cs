using gRPCIntercommunicationService.Protos;

namespace VaultAPI.Storage
{
    public class VaultActionsStorage
    {

        private List<StreamVaultCreationsResponse> _vaultCreations = new List<StreamVaultCreationsResponse>();

        private List<StreamVaultDeletionsResponse> _vaultDeletions = new List<StreamVaultDeletionsResponse>();  

        private List<StreamVaultUpdateResponse> _vaultUpdates = new List<StreamVaultUpdateResponse>();

        public void AddToVaultCreations(StreamVaultCreationsResponse vaultCreationResponse)
        {
            _vaultCreations.Add(vaultCreationResponse);
        }

        public void AddToVaultDeletions(StreamVaultDeletionsResponse vaultDeletionResponse)
        {
            _vaultDeletions.Add(vaultDeletionResponse);
        }

        public void AddToVaultUpdates(StreamVaultUpdateResponse vaultUpdateResponse)
        {
            _vaultUpdates.Add(vaultUpdateResponse);
        }

        public List<StreamVaultCreationsResponse> ReturnVaultCreations()
        {
            return _vaultCreations; 
        }

        public List<StreamVaultUpdateResponse> ReturnVaultUpdates()
        {
            return _vaultUpdates;
        }
        public List<StreamVaultDeletionsResponse> ReturnVaultDeletions()
        {
            return _vaultDeletions;
        }

        public void ClearVaultDeletionsDict()
        {
            _vaultDeletions.Clear();
        }

        public void ClearVaultCreationsDict()
        {
            _vaultCreations.Clear();
        }

        public void ClearVaultUpdatesDict()
        {
            _vaultUpdates.Clear();
        }



    }
}
