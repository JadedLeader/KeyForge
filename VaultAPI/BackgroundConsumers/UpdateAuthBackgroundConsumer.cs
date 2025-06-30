
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using Microsoft.EntityFrameworkCore.Query.Internal;
using VaultAPI.Repos;
using VaultAPI.Interfaces.RepoInterfaces;

namespace VaultAPI.BackgroundConsumers
{
    public class UpdateAuthBackgroundConsumer : BackgroundService
    {

        private readonly Auth.AuthClient _authClient;

        private readonly IServiceScopeFactory _serviceScope;

        private HashSet<string> _authUpdates = new HashSet<string>();

        public UpdateAuthBackgroundConsumer(Auth.AuthClient authClient, IServiceScopeFactory serviceScope)
        {
            _authClient = authClient;
            _serviceScope = serviceScope;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            using IServiceScope getScope = _serviceScope.CreateScope();

            IAuthRepo getAuthRepoLifetime = getScope.ServiceProvider.GetRequiredService<IAuthRepo>();

            while(!stoppingToken.IsCancellationRequested)
            {
                await UpdateAuths(getAuthRepoLifetime);
            }
        }


        private async Task UpdateAuths(IAuthRepo authRepo)
        {
            StreamAuthUpdatesRequest request = new StreamAuthUpdatesRequest();

            var handler = _authClient.StreamAuthKeyUpdates(request);

            var responseStream = handler.ResponseStream.ReadAllAsync();

            await foreach(StreamAuthUpdatesResponse authUpdate in responseStream)
            {
                if(authUpdate.UpdateType == UpdateType.ShortLivedUpdate)
                {
                    if(_authUpdates.Add(authUpdate.UpdateId))
                    {
                        await authRepo.UpdateShortLivedKey(Guid.Parse(authUpdate.AccountId), authUpdate.ShortLivedKey);
                    }
                }
                else if(authUpdate.UpdateType == UpdateType.LongLivedUpdate)
                {
                    if(_authUpdates.Add(authUpdate.UpdateId))
                    {
                        await authRepo.UpdateLongLivedKey(Guid.Parse(authUpdate.AccountId), authUpdate.LongLivedKey);
                    }
                }
            }
        }
    }
}
