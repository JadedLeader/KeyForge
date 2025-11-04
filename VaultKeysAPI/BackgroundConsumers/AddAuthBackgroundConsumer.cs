using Grpc.Core;
using gRPCIntercommunicationService;
using gRPCIntercommunicationService.Protos;
using KeyForgedShared.SharedDataModels;
using Serilog;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.BackgroundConsumers
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

            while (!stoppingToken.IsCancellationRequested)
            {
                await AddAuths(getAuthRepoLifetime);
            }
        }


        private async Task AddAuths(IAuthRepo authRepo)
        {
            try
            {
                var callOptions = new CallOptions().WithWaitForReady();
                StreamAuthCreationsRequest streamAuthCreations = new StreamAuthCreationsRequest();
                var handler = _authClient.StreamAuthCreations(streamAuthCreations, callOptions);
                var responseStream = handler.ResponseStream.ReadAllAsync();

                await foreach (var authCreationResponse in responseStream)
                {
                    if (!Guid.TryParse(authCreationResponse.AuthKey, out var authKey) || authKey == Guid.Empty)
                    {
                        Log.Warning("Invalid AuthKey in stream: {AuthKey}", authCreationResponse.AuthKey);
                        continue;
                    }

                    if (!Guid.TryParse(authCreationResponse.AccountId, out var accountId) || accountId == Guid.Empty)
                    {
                        Log.Warning("Invalid AccountId in stream: {AccountId}", authCreationResponse.AccountId);
                        continue;
                    }

                    if (_authCreations.Add(authKey))
                    {
                        var addAuthModelToDb = new AuthDataModel
                        {
                            AuthKey = authKey,
                            AccountId = accountId,
                            ShortLivedKey = authCreationResponse.ShortLivedKey ?? "",
                            LongLivedKey = authCreationResponse.LongLivedKey ?? ""
                        };

                        try
                        {
                            await authRepo.AddAsync(addAuthModelToDb);
                        }
                        catch (Exception dbEx)
                        {
                            Log.Error(dbEx, "Failed to add AuthDataModel to DB");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing AddAuths stream");
                // optionally delay a bit to avoid tight loop if the stream is failing continuously
                await Task.Delay(1000);
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

