using gRPCIntercommunicationService;

namespace AccountAPI.Storage
{
    public class StreamStorage
    {

        private readonly List<StreamAccountResponse> _accountCreationStream = new List<StreamAccountResponse>(); 

        public StreamStorage()
        {
            
        }

        public void AddToAccountCreationStream(StreamAccountResponse streamAccountResponse)
        {
            _accountCreationStream.Add(streamAccountResponse);
        }

        public void RemoveFromAccountCreationStream(StreamAccountResponse accountResponse)
        {
            _accountCreationStream.Remove(accountResponse);
        }

        public int AccountCreationStreamTotal()
        {
            return _accountCreationStream.Count;
        }

        public List<StreamAccountResponse> ReturnAccountCreationStream()
        {
            return _accountCreationStream;
        }



    }
}
