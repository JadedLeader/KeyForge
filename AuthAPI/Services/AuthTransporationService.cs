using AuthAPI.Interfaces.ServicesInterface;
using AuthAPI.TransporationStorage;
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using Serilog;

namespace AuthAPI.Services
{
    public class AuthTransporationService : Auth.AuthBase, IAuthTransportationService
    {

        private readonly AuthTransportationStorage _authTransporationStorage;

        public AuthTransporationService(AuthTransportationStorage authTransportationStorage)
        {
            _authTransporationStorage = authTransportationStorage;  
        }

        public override async Task StreamAuthCreations(StreamAuthCreationsRequest request, IServerStreamWriter<StreamAuthCreationsResponse> responseStream, ServerCallContext context)
        {
            foreach(var item in _authTransporationStorage.ReturnStreamAuthCreationsList())
            {
                Log.Information($"Sending auth item with auth key {item.AuthKey}");

                await responseStream.WriteAsync(item);
            }
        }

        public override async Task StreamAuthKeyUpdates(StreamAuthUpdatesRequest request, IServerStreamWriter<StreamAuthUpdatesResponse> responseStream, ServerCallContext context)
        {
            foreach(var item in _authTransporationStorage.ReturnStreamAuthUpdatesList())
            {
                if(item.UpdateType == UpdateType.ShortLivedUpdate)
                {
                    Log.Information($"Sending auth update of type {item.UpdateType} with new token of {item.ShortLivedKey}");
                }
                else if(item.UpdateType == UpdateType.LongLivedUpdate)
                {
                    Log.Information($"Sending auth update of type {item.UpdateType} with new token of {item.LongLivedKey}");
                }

                await responseStream.WriteAsync(item);
            }
        } 




    }
}
