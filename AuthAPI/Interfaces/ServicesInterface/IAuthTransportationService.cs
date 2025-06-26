using Grpc.Core;
using gRPCIntercommunicationService.Protos;

namespace AuthAPI.Interfaces.ServicesInterface
{
    public interface IAuthTransportationService
    {

        public Task StreamAuthCreationsToVaultApi(StreamAuthCreationsToVaultsRequest request, IServerStreamWriter<StreamAuthCreationsToVaultsResponse> responseStream, ServerCallContext context);

        public Task StreamAuthKeyUpdatesToVaultApi(StreamAuthUpdatesToVaultsRequest request, IServerStreamWriter<StreamAuthUpdatesToVaultsResponse> responseStream, ServerCallContext context);
    }
}
