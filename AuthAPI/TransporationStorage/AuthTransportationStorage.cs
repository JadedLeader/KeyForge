using gRPCIntercommunicationService.Protos;

namespace AuthAPI.TransporationStorage
{
    public class AuthTransportationStorage
    {

        private List<StreamAuthCreationsResponse> StreamAuthCreations = new();

        private List<StreamAuthUpdatesResponse> StreamAuthUpdates = new();


        public void AddToStreamAuthCreationsList(StreamAuthCreationsResponse authCreation)
        {
            StreamAuthCreations.Add(authCreation);
        }

        public void AddToStreamAuthUpdatesList(StreamAuthUpdatesResponse authUpdates)
        {
            StreamAuthUpdates.Add(authUpdates);
        }

        public List<StreamAuthCreationsResponse> ReturnStreamAuthCreationsList()
        {
            return StreamAuthCreations;
        }

        public List<StreamAuthUpdatesResponse> ReturnStreamAuthUpdatesList()
        {
            return StreamAuthUpdates;
        }

        public void ClearAuthCreations()
        {
            StreamAuthCreations.Clear();
        }

        public void ClearAuthUpdates()
        {
            StreamAuthUpdates.Clear();
        }

    }
}
