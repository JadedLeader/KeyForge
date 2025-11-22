using gRPCIntercommunicationService;

namespace AccountAPI.Storage
{
    public class StreamStorage
    {

        private readonly List<StreamAccountResponse> _accountCreationStream = new List<StreamAccountResponse>(); 

        private readonly List<Guid> _accountDeletionStream = new List<Guid>();

        public StreamStorage()
        {
            
        }

        public void AddToAccountCreationStream(StreamAccountResponse streamAccountResponse)
        {
            _accountCreationStream.Add(streamAccountResponse);
        }

        public void AddToAccountDeletionStream(Guid accountIdForDeletion)
        {
            _accountDeletionStream.Add(accountIdForDeletion);
        }

        public void RemoveFromAccountCreationStream(StreamAccountResponse accountResponse)
        {
            _accountCreationStream.Remove(accountResponse);
        }

        public int AccountCreationStreamTotal()
        {
            return _accountCreationStream.Count;
        }

        public int AccountDeletionStreamTotal()
        {
            return _accountDeletionStream.Count;
        }

        public List<StreamAccountResponse> ReturnAccountCreationStream()
        {
            return _accountCreationStream;
        }

        public List<Guid> ReturnAccountDeletionList()
        {
            return _accountDeletionStream;  
        }

        public void ClearAccountCreationStream()
        {
            _accountCreationStream.Clear();
        }

        public void ClearAccountDeletionsStream()
        {
            _accountDeletionStream.Clear();
        }



    }
}
