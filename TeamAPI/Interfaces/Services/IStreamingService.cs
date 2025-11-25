using Grpc.Core;
using gRPCIntercommunicationService;
using System.Collections.Immutable;
using TeamAPI.Storage;

namespace TeamAPI.Interfaces.Services
{
    public interface IStreamingService
    {

        public Task StreamTeamCreations(StreamTeamCreationRequest request, IServerStreamWriter<StreamTeamCreationResponse> responseStream, ServerCallContext context);


        public Task StreamTeamDeletions(StreamTeamDeletionRequest request, IServerStreamWriter<StreamTeamDeletionResponse> responseStream, ServerCallContext context);


        public Task StreamTeamUpdates(StreamTeamUpdateRequest request, IServerStreamWriter<StreamTeamUpdateResponse> responseStream, ServerCallContext context);

    }
}
