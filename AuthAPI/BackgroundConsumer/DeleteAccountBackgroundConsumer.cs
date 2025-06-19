
using AuthAPI.Interfaces.RepoInterface;
using Grpc.Core;
using gRPCIntercommunicationService;
using Serilog;
using System.Threading;

namespace AuthAPI.BackgroundConsumer
{
    public class DeleteAccountBackgroundConsumer : BackgroundService
    {

        private readonly HashSet<Guid> _seenIds = new();

        private readonly Account.AccountClient _Accountclient;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        public DeleteAccountBackgroundConsumer(Account.AccountClient accountClient, IServiceScopeFactory serviceScopeFactory)
        {
            _Accountclient = accountClient;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                StreamAccountDeleteRequest streamAccountDeletions = new StreamAccountDeleteRequest();

                using var stream = _Accountclient.StreamAccountDeletions(streamAccountDeletions, null, null, stoppingToken);

                await ReceiveAccountDeletionStream(stoppingToken, stream);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task ReceiveAccountDeletionStream(CancellationToken cancellationToken, AsyncServerStreamingCall<StreamAccountDeleteResponse> stream)
        {
            await foreach(var accountDeletion in stream.ResponseStream.ReadAllAsync(cancellationToken))
            {
                if(_seenIds.Add(Guid.Parse(accountDeletion.AccountId)))
                {
                    IServiceScope freshScope = _serviceScopeFactory.CreateScope();

                    IAuthRepo authRepo = freshScope.ServiceProvider.GetRequiredService<IAuthRepo>();

                    await authRepo.RemoveAccountFromTablesViaId(Guid.Parse(accountDeletion.AccountId));
                }
            }
        }

   
    }
}
