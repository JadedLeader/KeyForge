using AccountAPI.DataModel;
using AuthAPI.DataModel;
using AuthAPI.Interfaces.RepoInterface;
using AuthAPI.Interfaces.ServicesInterface;
using Grpc.Core;
using gRPCIntercommunicationService.Protos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Pipes;

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

        /// <summary>
        /// Generates both a long lived and short lived key, reflective of a users first sign-on to the platform
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateAuthAccountResponse> CreateAuthAccount(CreateAuthAccountRequest request)
        {
            AuthDataModel existingAuth = await CheckForExistingAuth(Guid.Parse(request.AccountId));

            AccountDataModel? existingAccount = await CheckForExistingAccount(Guid.Parse(request.AccountId));

            CreateAuthAccountResponse serverResponse = new CreateAuthAccountResponse();

            string shortLivedToken = _tokenGeneratorService.GenerateShortLivedToken(existingAccount.AccountId.ToString());
            string longLivedToken = _tokenGeneratorService.GenerateLongLivedToken(existingAccount.AccountId.ToString());

            AuthDataModel creatingAuthModel = CreateAuthToAuthModel(request, existingAccount, shortLivedToken, longLivedToken);

            if (existingAccount.AccountId == Guid.Empty)
            {
                Log.Warning($"No account can be found, generating authentication token failed");

                serverResponse.AccountId = "";
                serverResponse.Successful = false;
                serverResponse.ShortLivedToken = "";
                serverResponse.LongLivedToken = "";

                return serverResponse;
            }
            else if(existingAccount.AccountId != Guid.Empty && existingAuth.AccountId == Guid.Empty)
            {
                Log.Information($"{existingAccount.AccountId} generated short lived token: {shortLivedToken}");
                Log.Information($"{existingAccount.AccountId} generated long lived token: {longLivedToken}");

                await _authRepo.AddAuthToTable(creatingAuthModel);

            }
            else if(existingAccount.AccountId != Guid.Empty && existingAuth.AccountId != Guid.Empty)
            {
                Log.Information($"{existingAccount.AccountId} generated short lived token: {shortLivedToken}");
                Log.Information($"{existingAccount.AccountId} generated long lived token: {longLivedToken}");

                await _authRepo.UpdateExistingAuthKeys(existingAuth, longLivedToken, shortLivedToken);
            }

            serverResponse.AccountId = creatingAuthModel.AccountId.ToString();
            serverResponse.ShortLivedToken = shortLivedToken;
            serverResponse.LongLivedToken = longLivedToken;
            serverResponse.Successful = true;

            return serverResponse;

        }

        /// <summary>
        /// Refreshes the current long lived key token as long as an active auth record exists
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Refreshes the short lived key, as long as there's an already existing auth record for the account and the long lived key is still valid
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<RefreshShortLivedTokenResponse> RefreshShortLivedToken(RefreshShortLivedTokenRequest request)
        {
            AuthDataModel existingAuth = await CheckForExistingAuth(Guid.Parse(request.AccountId));

            RefreshShortLivedTokenResponse serverResponse = new RefreshShortLivedTokenResponse();

            string? currentLongLivedKey = existingAuth.LongLivedKey;

            if (existingAuth.AccountId == Guid.Empty || existingAuth.LongLivedKey == string.Empty)
            {
                Log.Error($"No valid auth has been previously setup or the long lived key is currently invalid");

                serverResponse.AccountId = request.AccountId;
                serverResponse.Successful = false;
                serverResponse.RefreshedToken = "";

                return serverResponse;
            }


            bool isLongKeyValid = IsLongLivedKeyValid(currentLongLivedKey);


            string refreshedShortLivedToken = _tokenGeneratorService.GenerateShortLivedToken(existingAuth.AccountId.ToString());

            AuthDataModel authRecord = await _authRepo.UpdateShortLivedToken(existingAuth, refreshedShortLivedToken); 

            serverResponse.AccountId = existingAuth.AccountId.ToString();
            serverResponse.Successful = true;
            serverResponse.RefreshedToken = refreshedShortLivedToken;

            return serverResponse;
        }

        /// <summary>
        /// Revokes a long lived token, this removes the long lived token and the short lived token that are connected to a users account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RevokeLongLivedTokenResponse> RevokeLongLivedToken(RevokeLongLivedTokenRequest request)
        {
            AuthDataModel existingAuth = await _authRepo.CheckForExistingAuth(Guid.Parse(request.AccountId));

            RevokeLongLivedTokenResponse serverResponse = new RevokeLongLivedTokenResponse();

            if(existingAuth.AccountId == Guid.Empty)
            {
                Log.Error($"No valid auth has previously been setup or registered"); 

                serverResponse.AccountId = request.AccountId;
                serverResponse.Successful = false;

                return serverResponse;
            }

            AuthDataModel revokingKeys = await _authRepo.RevokeLongLivedToken(existingAuth);

            Log.Information($"{this.GetType().Namespace} Keys have been revoked for account with ID {existingAuth.AccountId}");

            serverResponse.AccountId = existingAuth.AccountId.ToString();
            serverResponse.Successful = true;

            return serverResponse;
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

        private bool IsLongLivedKeyValid(string currentLongLivedKey)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();    

            if(currentLongLivedKey == string.Empty)
            {
                return false;
            }

            var longLivedKey = handler.ReadToken(currentLongLivedKey);

            DateTime validToDate = longLivedKey.ValidTo;

            if(validToDate < DateTime.Now)
            {
                return false; 
            }
            
            return true;
        }







    }
}
