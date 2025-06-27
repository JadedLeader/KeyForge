
using AuthAPI.DataModel;
using AuthAPI.Interfaces.RepoInterface;
using Serilog;
using Grpc.Core;
using gRPCIntercommunicationService;
using System.Threading;

namespace AuthAPI.BackgroundConsumer
{
    public class AccountBackgroundConsumer :  BackgroundService
    {
        private readonly HashSet<Guid> _seenIds = new();

        private readonly Account.AccountClient _accountClient;

        private readonly IServiceScopeFactory _scopeFactory;

        public AccountBackgroundConsumer(Account.AccountClient client, IServiceScopeFactory scopeFactory)
        {
            _accountClient  = client;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information($"account background consumer loaded");

            while (!stoppingToken.IsCancellationRequested)
            {
                StreamAccountRequest request = new StreamAccountRequest();

                using var call = _accountClient.StreamAccount(request, null, null, stoppingToken);

                await ConsumeAccountChannelStream(stoppingToken, call);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task ConsumeAccountChannelStream(CancellationToken cancellationToken, AsyncServerStreamingCall<StreamAccountResponse> call)
        {
            
            Log.Information($"Nothing in the queue, looping");

            await foreach (var resp in call.ResponseStream.ReadAllAsync(cancellationToken))
            { 
                Log.Information($"current item in response stream {resp}");

                AccountDataModel mapToAccountModel = StreamResponseToAccountModel(resp);

                if(_seenIds.Add(mapToAccountModel.AccountId))
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();

                    IAuthRepo authRepo = scope.ServiceProvider.GetRequiredService<IAuthRepo>();

                    await authRepo.AddAccountToTable(mapToAccountModel);
                }
            }
        }

        private AccountDataModel StreamResponseToAccountModel(StreamAccountResponse streamAccountResponse)
        {
            AccountDataModel newAccountDataModel = new AccountDataModel
            {
                AccountId = Guid.Parse(streamAccountResponse.AccountId),
                Username = streamAccountResponse.Username,
                Password = streamAccountResponse.Password,
                Email = streamAccountResponse.Email,
                AuthorisationLevel = (AuthRoles)streamAccountResponse.AuthRole,
                AccountCreated = DateTime.Parse(streamAccountResponse.AccountCreated),
            };

            return newAccountDataModel;
        }
       
    }
}
