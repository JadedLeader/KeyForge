using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.Generics
{
    public abstract class GenericGrpcConsumer<TGrpcResponse, TModel> : BackgroundService
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;

        protected GenericGrpcConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected abstract TModel MapToType(TGrpcResponse responseType);
        protected abstract IAsyncEnumerable<TGrpcResponse> OpenStream();
        protected abstract Task HandleMessage(IServiceProvider service,  TModel model);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach(TGrpcResponse response in OpenStream())
            {
                Log.Information($"{typeof(TGrpcResponse)}: {typeof(TModel)} received");

                using IServiceScope scope = _serviceScopeFactory.CreateScope();

                var services = scope.ServiceProvider;

                var model = MapToType(response);

                if(model == null)
                {
                    continue;
                }

                await HandleMessage(services, model);
            }
        }

    }
}
