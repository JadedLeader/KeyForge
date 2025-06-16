using AccountAPI.DataModel;
using AuthAPI.DataModel;
using AuthAPI.Interfaces.RepoInterface;
using AuthAPI.Interfaces.ServicesInterface;
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AuthAPI.Services 
{
    public class AuthService : Auth.AuthBase, IAuthService 
    {

        private readonly ITokenGeneratorService _tokenGeneratorService;

        private readonly IAuthRepo _authRepo;

        public AuthService(ITokenGeneratorService tokenGeneratorService, IAuthRepo authRepo)
        {
            _tokenGeneratorService = tokenGeneratorService;
            _authRepo = authRepo;
        }

        public async Task<CreateAuthAccountResponse> CreateAuthAccount(CreateAuthAccountRequest request)
        {
            AccountDataModel? existingAccount = await CheckForExistingAccount(Guid.Parse(request.AccountId));

            CreateAuthAccountResponse serverResponse = new CreateAuthAccountResponse();

            if(existingAccount.AccountId == Guid.Empty)
            {
                Log.Warning($"No account can be found, generating authentication token failed");

                serverResponse.AccountId = "";
                serverResponse.Successful = false;
                serverResponse.ShortLivedToken = "";
                serverResponse.LongLivedToken = "";

                return serverResponse;
            }

            string shortLivedToken = _tokenGeneratorService.GenerateShortLivedToken(existingAccount.AccountId.ToString()); 
            string longLivedToken = _tokenGeneratorService.GenerateLongLivedToken(existingAccount.AccountId.ToString());

            Log.Information($"{existingAccount.AccountId} generated short lived token: {shortLivedToken}");
            Log.Information($"{existingAccount.AccountId} generated long lived token: {longLivedToken}");

            AuthDataModel creatingAuthModel = CreateAuthToAuthModel(request, existingAccount, shortLivedToken, longLivedToken);

            await _authRepo.AddAuthToTable(creatingAuthModel);

            serverResponse.AccountId = creatingAuthModel.AccountId.ToString();
            serverResponse.ShortLivedToken = shortLivedToken;
            serverResponse.LongLivedToken= longLivedToken;
            serverResponse.Successful = true;

            return serverResponse;
        }

        public async Task<RefreshLongLivedTokenResponse> RefreshLongLivedToken(RefreshLongLivedTokenRequest request)
        {
            
            AuthDataModel existingAuth = await CheckForExistingAuth(Guid.Parse(request.AccountId));

            RefreshLongLivedTokenResponse serverResponse = new RefreshLongLivedTokenResponse();

            if(existingAuth.AuthKey == Guid.Empty || existingAuth.AccountId == Guid.Empty)
            {
                Log.Error("No valid auth has been previously setup, please ensure basic auth configuration has been carried out first");

                serverResponse.AccountId = request.AccountId;
                serverResponse.Successful = false;
                serverResponse.RefreshedToken = "";

                return serverResponse;
            }

            string refreshedLongLivedToken = _tokenGeneratorService.GenerateLongLivedToken(existingAuth.AccountId.ToString());

            AuthDataModel updatedAuthModel = await _authRepo.UpdateLongLivedToken(existingAuth, refreshedLongLivedToken);

            serverResponse.AccountId = updatedAuthModel.AccountId.ToString();
            serverResponse.Successful = true;
            serverResponse.RefreshedToken = refreshedLongLivedToken;

            return serverResponse;
        }

        public async Task<RefreshShortLivedTokenResponse> RefreshShortLivedToken(RefreshShortLivedTokenRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        private async Task<AccountDataModel> CheckForExistingAccount(Guid accountId)
        {
            AccountDataModel account = await _authRepo.CheckForExistingAccount(accountId);

            if(account.AccountId == Guid.Empty)
            {
                return new AccountDataModel();
            }

            return account;

        }

        private async Task<AuthDataModel> CheckForExistingAuth(Guid accountId)
        {
            AuthDataModel auth = await _authRepo.CheckForExistingAuth(accountId);

            if(auth.AccountId == Guid.Empty || auth.AuthKey == Guid.Empty)
            {
                return new AuthDataModel();
            }

            return auth;
        }

        private AuthDataModel CreateAuthToAuthModel(CreateAuthAccountRequest createAuthRequest, AccountDataModel account, string shortLivedKey, string longLivedKey)
        {
            AuthDataModel authModel = new AuthDataModel
            {
                AccountId = Guid.Parse(createAuthRequest.AccountId),
                Account = account,
                AuthKey = Guid.NewGuid(),
                ShortLivedKey = shortLivedKey,
                LongLivedKey = longLivedKey
            }; 

            return authModel;
        }







    }
}
