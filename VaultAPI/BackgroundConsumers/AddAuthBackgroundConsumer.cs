using KeyForgedShared.SharedDataModels;
using Grpc.Core;
using gRPCIntercommunicationService;
using gRPCIntercommunicationService.Protos;
using VaultAPI.Interfaces.RepoInterfaces;

namespace VaultAPI.BackgroundConsumers
{
    public class AddAuthBackgroundConsumer : BackgroundService
    {

        private readonly Auth.AuthClient _authClient;

        private readonly IServiceScopeFactory _serviceScope;

        private HashSet<Guid> _authCreations = new HashSet<Guid>();

        public AddAuthBackgroundConsumer(Auth.AuthClient authClient, IServiceScopeFactory serviceScope)
        {
            _authClient = authClient;
            _serviceScope = serviceScope;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using IServiceScope createScope = _serviceScope.CreateScope();

            IAuthRepo getAuthRepoLifetime = createScope.ServiceProvider.GetRequiredService<IAuthRepo>();

            while(!stoppingToken.IsCancellationRequested)
            {
                await AddAuths(getAuthRepoLifetime);
            }
        }


        private async Task AddAuths(IAuthRepo authRepo)
        {

            var callOptions = new CallOptions().WithWaitForReady();
            StreamAuthCreationsRequest streamAuthCreations = new StreamAuthCreationsRequest();

            var handler = _authClient.StreamAuthCreations(streamAuthCreations, callOptions);

            var responseStream = handler.ResponseStream.ReadAllAsync();

            await foreach(StreamAuthCreationsResponse authCreationResponse in responseStream)
            {

                if(_authCreations.Add(Guid.Parse(authCreationResponse.AuthKey)))
                {

                    AuthDataModel addAuthModelToDb = MapStreamAuthCreationToAuthModel(authCreationResponse);

                    await authRepo.AddAsync(addAuthModelToDb);
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
                

            };

            return newAuthDataModel;

            
        }

        
    }
}

