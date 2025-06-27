
using AccountAPI.DataModel;
using AuthAPI.DataModel;
using Grpc.Core;
using gRPCIntercommunicationService;
using gRPCIntercommunicationService.Protos;
using VaultAPI.Repos;

namespace VaultAPI.BackgroundConsumers
{
    public class AddAuthBackgroundConsumer : BackgroundService
    {

        private readonly Auth.AuthClient _authClient;

        private readonly AuthRepo _authRepo;

        private HashSet<StreamAuthCreationsResponse> _authCreations = new HashSet<StreamAuthCreationsResponse>();

        public AddAuthBackgroundConsumer(Auth.AuthClient authClient, AuthRepo authRepo)
        {
            _authClient = authClient;
            _authRepo = authRepo;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await AddAuths();
            }
        }


        private async Task AddAuths()
        {

            StreamAuthCreationsRequest streamAuthCreations = new StreamAuthCreationsRequest();

            var handler = _authClient.StreamAuthCreations(streamAuthCreations);

            var responseStream = handler.ResponseStream.ReadAllAsync();

            await foreach(StreamAuthCreationsResponse authCreationResponse in responseStream)
            {

                if(_authCreations.Add(authCreationResponse))
                {

                    AuthDataModel addAuthModelToDb = MapStreamAuthCreationToAuthModel(authCreationResponse);

                    await _authRepo.AddAsync(addAuthModelToDb);
                }

            }
          
        }


        private AuthDataModel MapStreamAuthCreationToAuthModel(StreamAuthCreationsResponse streamAuthCreation)
        {
            AuthDataModel newAuthDataModel = new AuthDataModel
            {
                AuthKey = Guid.Parse(streamAuthCreation.AuthKey),
                AccountId = Guid.Parse(streamAuthCreation.AccountId),
                ShortLivedKey = streamAuthCreation.ShortLivedKey,
                LongLivedKey = streamAuthCreation.LongLivedKey,
                Account = new AccountDataModel
                {
                    AccountId = Guid.Parse(streamAuthCreation.Account.AccountId),
                    Username = streamAuthCreation.Account.Username, 
                    Password = streamAuthCreation.Account.Password,
                    Email = streamAuthCreation.Account.Email,
                    AccountCreated = DateTime.Parse(streamAuthCreation.Account.AccountCreated), 
                    AuthorisationLevel = (AccountAPI.DataModel.AuthRoles)streamAuthCreation.Account.Authroles

                }

            };

            return newAuthDataModel;

            
        }

        
    }
}

