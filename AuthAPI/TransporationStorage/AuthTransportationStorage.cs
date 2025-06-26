using gRPCIntercommunicationService.Protos;

namespace AuthAPI.TransporationStorage
{
    public class AuthTransportationStorage
    {

        private List<StreamAuthCreationsToVaultsResponse> StreamAuthCreations = new();

        private List<StreamAuthUpdatesToVaultsResponse> StreamAuthUpdates = new();


        public void AddToStreamAuthCreationsList(StreamAuthCreationsToVaultsResponse authCreation)
        {
            StreamAuthCreations.Add(authCreation);
        }

        public void AddToStreamAuthUpdatesList(StreamAuthUpdatesToVaultsResponse authUpdates)
        {
            StreamAuthUpdates.Add(authUpdates);
        }

        public List<StreamAuthCreationsToVaultsResponse> ReturnStreamAuthCreationsList()
        {
            return StreamAuthCreations;
        }

        public List<StreamAuthUpdatesToVaultsResponse> ReturnStreamAuthUpdatesList()
        {
            return StreamAuthUpdates;
        }

    }
}
