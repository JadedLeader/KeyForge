
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Serilog;
using VaultAPI.Interfaces.RepoInterfaces;
using VaultAPI.Repos;

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
            
            while(!stoppingToken.IsCancellationRequested)
            {
                await UpdateAuths();
            }
        }

        private async Task UpdateAuths()
        {

            try
            {
                var callOptions = new CallOptions().WithWaitForReady();

                StreamAuthUpdatesRequest request = new StreamAuthUpdatesRequest();

                var handler = _authClient.StreamAuthKeyUpdates(request, callOptions);

                var responseStream = handler.ResponseStream.ReadAllAsync();

                await foreach (StreamAuthUpdatesResponse authUpdate in responseStream)
                {
                    using IServiceScope getScope = _serviceScope.CreateScope();

                    IAuthRepo authRepo = getScope.ServiceProvider.GetRequiredService<IAuthRepo>();

                    if (authUpdate.UpdateType == UpdateType.ShortLivedUpdate)
                    {
                        if (_authUpdates.Add(authUpdate.UpdateId))
                        {
                            await authRepo.UpdateShortLivedKey(Guid.Parse(authUpdate.AccountId), authUpdate.ShortLivedKey);
                        }
                    }
                    else if (authUpdate.UpdateType == UpdateType.LongLivedUpdate)
                    {
                        if (_authUpdates.Add(authUpdate.UpdateId))
                        {
                            await authRepo.UpdateLongLivedKey(Guid.Parse(authUpdate.AccountId), authUpdate.LongLivedKey);
                        }
                    }
                }
            }
            catch (RpcException ex)
            {
                Log.Error(ex, "gRPC stream dropped — reconnecting...");
                await Task.Delay(1000);  
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception in UpdateAuth background consumer");
                await Task.Delay(1000);
            }
        }
    }
}
