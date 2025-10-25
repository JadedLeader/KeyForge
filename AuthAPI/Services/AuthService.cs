using KeyForgedShared.SharedDataModels;
using AuthAPI.Interfaces.RepoInterface;
using AuthAPI.Interfaces.ServicesInterface;
using AuthAPI.TransporationStorage;
using Azure;
using Grpc.Core;
using gRPCIntercommunicationService;
using gRPCIntercommunicationService.Protos;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using KeyForgedShared.Helpers;
using KeyForgedShared.Interfaces;

namespace AuthAPI.Services 
{
    public class AuthService : Auth.AuthBase, IAuthService 
    {

        private readonly ITokenGeneratorService _tokenGeneratorService;

        private readonly IAuthRepo _authRepo;

        private readonly AuthTransportationStorage _transportationStorage;

        private readonly IJwtHelper _jwtHelper;

        public AuthService(ITokenGeneratorService tokenGeneratorService, IAuthRepo authRepo, AuthTransportationStorage transportationStorage, IJwtHelper jwtHelper)
        {
            _tokenGeneratorService = tokenGeneratorService;
            _authRepo = authRepo;
            _transportationStorage = transportationStorage;
            _jwtHelper = jwtHelper;
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

            string shortLivedToken = _tokenGeneratorService.GenerateShortLivedToken(existingAccount.AccountId.ToString(), existingAccount.AuthorisationLevel.ToString());
            string longLivedToken = _tokenGeneratorService.GenerateLongLivedToken(existingAccount.AccountId.ToString(), existingAccount.AuthorisationLevel.ToString());

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

                StreamAuthCreationsResponse mapToStorage = MapAuthModelToStreamAuthcreations(creatingAuthModel);

                _transportationStorage.AddToStreamAuthCreationsList(mapToStorage);

                serverResponse.AccountId = creatingAuthModel.AccountId.ToString();
                serverResponse.ShortLivedToken = shortLivedToken;
                serverResponse.LongLivedToken = longLivedToken;
                serverResponse.Successful = true;

            }
            else if(existingAccount.AccountId != Guid.Empty && existingAuth.AccountId != Guid.Empty)
            {
                Log.Information($"Account exists and already has allocated auth tokens");

                serverResponse.AccountId = existingAccount.AccountId.ToString(); 
                serverResponse.Successful = true;
                serverResponse.ShortLivedToken = existingAuth.ShortLivedKey;
                serverResponse.LongLivedToken = existingAuth.LongLivedKey;
                serverResponse.Details = "Keys already allocated";
            }

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

            string? retrieveRoleFromCurrentToken = _jwtHelper.ReturnRoleFromToken(existingAuth.LongLivedKey);

            RefreshLongLivedTokenResponse serverResponse = new RefreshLongLivedTokenResponse();

            if(existingAuth.AuthKey == Guid.Empty || existingAuth.AccountId == Guid.Empty)
            {
                Log.Error("No valid auth has been previously setup, please ensure basic auth configuration has been carried out first");

                serverResponse.AccountId = request.AccountId;
                serverResponse.Successful = false;
                serverResponse.RefreshedToken = "";

                return serverResponse;
            }

            string refreshedLongLivedToken = _tokenGeneratorService.GenerateLongLivedToken(existingAuth.AccountId.ToString(), retrieveRoleFromCurrentToken);

            StreamAuthUpdatesResponse mapToStreamUpdates = MapToStreamAuthUpdates(existingAuth.AccountId.ToString(), null, refreshedLongLivedToken, UpdateType.LongLivedUpdate);

            _transportationStorage.AddToStreamAuthUpdatesList(mapToStreamUpdates);

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

            string? retrieveRoleFromCurrentToken = _jwtHelper.ReturnRoleFromToken(existingAuth.LongLivedKey);

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

            bool isLongKeyValid = _jwtHelper.IsLongLivedKeyValid(currentLongLivedKey);

            string refreshedShortLivedToken = _tokenGeneratorService.GenerateShortLivedToken(existingAuth.AccountId.ToString(), retrieveRoleFromCurrentToken);

            StreamAuthUpdatesResponse mapToStreamUpdates = MapToStreamAuthUpdates(existingAuth.AccountId.ToString(), refreshedShortLivedToken, null, UpdateType.ShortLivedUpdate);

            _transportationStorage.AddToStreamAuthUpdatesList(mapToStreamUpdates);

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

        /// <summary>
        /// Incredibly simple login, checks for basic equality between the username and password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            
            AccountDataModel retrievingAccount = await _authRepo.CheckForExistingAccountViaUsername(request.Username);

            LoginResponse serverResponse = new LoginResponse(); 

            if(retrievingAccount.AccountId == Guid.Empty)
            {
                Log.Warning($"No account exists with username {request.Username}");

                serverResponse.Username = request.Username;
                serverResponse.AccountId = "";
                serverResponse.Successful = false;

                return serverResponse;
            }

            bool validatingUsernameMatch = ValidatingUsernameMatch(request.Username, retrievingAccount.Username);
            bool validatingPassMatch = ValidatingPasswordMatch(request.Password, retrievingAccount.Password);

            if (!validatingPassMatch || !validatingUsernameMatch)
            {
                serverResponse.Username = request.Username;
                serverResponse.AccountId = ""; 
                serverResponse.Successful = false;

                return serverResponse;
            }

            serverResponse.Username = retrievingAccount.Username;
            serverResponse.AccountId = retrievingAccount.AccountId.ToString();
            serverResponse.Successful = true;

            return serverResponse;



        }

        public async Task<ReinstateAuthKeyResponse> ReinstantiateAuthKey(ReinstateAuthKeyRequest request)
        {

            AuthDataModel checkingForExistingKeys = await _authRepo.CheckForExistingAuth(Guid.Parse(request.AccountId));

            AccountDataModel? getRole = await _authRepo.RetrieveRoleFromAccount(checkingForExistingKeys.AccountId);

            ReinstateAuthKeyResponse serverResponse = new ReinstateAuthKeyResponse();

            if (checkingForExistingKeys.AccountId == Guid.Empty)
            {
                Log.Information($"No account can be found to re-instate keys for");

                serverResponse.AccountId = request.AccountId;
                serverResponse.ShortLivedKey = "";
                serverResponse.LongLivedKey = "";
                serverResponse.Successful = false;

                return serverResponse;

            }

            string generateShortLivedKey = _tokenGeneratorService.GenerateShortLivedToken(checkingForExistingKeys.AccountId.ToString(), getRole.ToString());

            string generateLongLivedKey = _tokenGeneratorService.GenerateLongLivedToken(checkingForExistingKeys.Account.ToString(), getRole.ToString());

            await _authRepo.UpdateExistingAuthKeys(checkingForExistingKeys, generateLongLivedKey, generateShortLivedKey);

            serverResponse.AccountId = checkingForExistingKeys.AccountId.ToString();
            serverResponse.ShortLivedKey = generateShortLivedKey;
            serverResponse.LongLivedKey= generateLongLivedKey;
            serverResponse.Successful = true;
         
            return serverResponse;
        }

        public async Task<SilentShortLivedTokenRefreshResponse> SilentTokenCycle(SilentShortLivedTokenRefreshRequest request, string longLivedToken)
        {

            SilentShortLivedTokenRefreshResponse serverResponse = new SilentShortLivedTokenRefreshResponse();

            string? accountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(longLivedToken);
            string? accountRoleFromToken = _jwtHelper.ReturnRoleFromToken(longLivedToken);

            if(accountIdFromToken == string.Empty || accountRoleFromToken == string.Empty)
            {
                serverResponse.RefreshedShortLivedToken = "";
                serverResponse.Successful = false; 

                return serverResponse;
            }

            string? refreshedShortLivedToken = _tokenGeneratorService.GenerateShortLivedToken(accountIdFromToken, accountRoleFromToken);

            StreamAuthUpdatesResponse mapToStreamUpdates = MapToStreamAuthUpdates(accountIdFromToken, refreshedShortLivedToken, null, UpdateType.ShortLivedUpdate);

            _transportationStorage.AddToStreamAuthUpdatesList(mapToStreamUpdates);

            AuthDataModel authAccount = await _authRepo.CheckForExistingAuthViaAccountId(Guid.Parse(accountIdFromToken));

            if (authAccount.AuthKey == Guid.Empty)
            {
                Log.Information($"No account can be found with the provided account ID");

                serverResponse.RefreshedShortLivedToken = "";
                serverResponse.Successful = false; 

                return serverResponse;
            }

            await _authRepo.UpdateShortLivedToken(authAccount, refreshedShortLivedToken);

            serverResponse.RefreshedShortLivedToken = refreshedShortLivedToken;
            serverResponse.Successful = true;

            return serverResponse;



        }

        public override async Task StreamAuthCreations(StreamAuthCreationsRequest request, IServerStreamWriter<StreamAuthCreationsResponse> responseStream, ServerCallContext context)
        {
            foreach (var item in _transportationStorage.ReturnStreamAuthCreationsList())
            {
                Log.Information($"Sending auth item with auth key {item.AuthKey}");

                await responseStream.WriteAsync(item);
            }
        }

        public override async Task StreamAuthKeyUpdates(StreamAuthUpdatesRequest request, IServerStreamWriter<StreamAuthUpdatesResponse> responseStream, ServerCallContext context)
        {
            foreach (var item in _transportationStorage.ReturnStreamAuthUpdatesList())
            {
                if (item.UpdateType == UpdateType.ShortLivedUpdate)
                {
                    Log.Information($"Sending auth update of type {item.UpdateType} with new token of {item.ShortLivedKey}");
                }
                else if (item.UpdateType == UpdateType.LongLivedUpdate)
                {
                    Log.Information($"Sending auth update of type {item.UpdateType} with new token of {item.LongLivedKey}");
                }

                await responseStream.WriteAsync(item);
            }
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

        private bool ValidatingPasswordMatch(string password, string hashedPassword)
        {

            bool match = BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);

            if(!match)
            {
                return false;
            }


            return true;
        }

        private bool ValidatingUsernameMatch(string username, string usernameFromDb)
        {
            if(username != usernameFromDb)
            {
                return false;
            }

            return true;
        }

        private StreamAuthCreationsResponse MapAuthModelToStreamAuthcreations(AuthDataModel authModel)
        {
          
            StreamAuthCreationsResponse newVaultResponse = new StreamAuthCreationsResponse
            {
                AuthKey = authModel.AuthKey.ToString(),
                AccountId = authModel.AccountId.ToString(),
                ShortLivedKey = authModel.ShortLivedKey,
                LongLivedKey = authModel.LongLivedKey, 
              
            };

            return newVaultResponse;
        }

        private StreamAuthUpdatesResponse MapToStreamAuthUpdates(string accountId, string? shortLivedKey, string? longLivedKey, UpdateType updateType)
        {
            StreamAuthUpdatesResponse streamAuthUpdate = new StreamAuthUpdatesResponse
            {
                UpdateId = Guid.NewGuid().ToString(),
                AccountId = accountId,
                ShortLivedKey = shortLivedKey,
                LongLivedKey = longLivedKey,
                UpdateType = updateType
            };

            return streamAuthUpdate;
        }

        







    }
}
