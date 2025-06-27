using Grpc.Core;
using gRPCIntercommunicationService.Protos;

namespace AuthAPI.Interfaces.ServicesInterface
{
    public interface IAuthTransportationService
    {

        public Task StreamAuthCreations(StreamAuthCreationsRequest request, IServerStreamWriter<StreamAuthCreationsResponse> responseStream, ServerCallContext context);

        public Task StreamAuthKeyUpdates(StreamAuthUpdatesRequest request, IServerStreamWriter<StreamAuthUpdatesResponse> responseStream, ServerCallContext context);
    }
}
