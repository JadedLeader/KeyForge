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
using System.Collections.Immutable;
using KeyForgedShared.DTO_s.AuthDTO_s;
using KeyForgedShared.ReturnTypes.Auth;

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
        public async Task<CreateAuthReturn> CreateAuthAccount(CreateAuthDto request)
        {
            AuthDataModel existingAuth = await CheckForExistingAuth(Guid.Parse(request.AccountId));

            AccountDataModel? existingAccount = await CheckForExistingAccount(Guid.Parse(request.AccountId));

            CreateAuthReturn serverResponse = new CreateAuthReturn();

            string shortLivedToken = _tokenGeneratorService.GenerateShortLivedToken(existingAccount.Id.ToString(), existingAccount.AuthorisationLevel.ToString());
            string longLivedToken = _tokenGeneratorService.GenerateLongLivedToken(existingAccount.Id.ToString(), existingAccount.AuthorisationLevel.ToString());

            AuthDataModel creatingAuthModel = CreateAuthToAuthModel(request, existingAccount, shortLivedToken, longLivedToken);

            if (existingAccount.Id == Guid.Empty)
            {
                Log.Warning($"No account can be found, generating authentication token failed");

                serverResponse.AccountId = "";
                serverResponse.Success = false;
                serverResponse.ShortLivedToken = "";
                serverResponse.LongLivedToken = "";

                return serverResponse;
            }
            else if(existingAccount.Id != Guid.Empty && existingAuth.AccountId == Guid.Empty)
            {
                Log.Information($"{existingAccount.Id} generated short lived token: {shortLivedToken}");
                Log.Information($"{existingAccount.Id} generated long lived token: {longLivedToken}");

                await _authRepo.AddAuthToTable(creatingAuthModel);

                StreamAuthCreationsResponse mapToStorage = MapAuthModelToStreamAuthcreations(creatingAuthModel);

                _transportationStorage.AddToStreamAuthCreationsList(mapToStorage);

                serverResponse.AccountId = creatingAuthModel.AccountId.ToString();
                serverResponse.ShortLivedToken = shortLivedToken;
                serverResponse.LongLivedToken = longLivedToken;
                serverResponse.Success = true;

            }
            else if(existingAccount.Id != Guid.Empty && existingAuth.AccountId != Guid.Empty)
            {
                Log.Information($"Account exists and already has allocated auth tokens");

                serverResponse.AccountId = existingAccount.Id.ToString(); 
                serverResponse.Success = true;
                serverResponse.ShortLivedToken = existingAuth.ShortLivedKey;
                serverResponse.LongLivedToken = existingAuth.LongLivedKey;
            }

            return serverResponse;

        }

        /// <summary>
        /// Refreshes the current long lived key token as long as an active auth record exists
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RefreshLongLivedTokenReturn> RefreshLongLivedToken(RefreshLongLivedTokenDto request)
        {
            
            AuthDataModel existingAuth = await CheckForExistingAuth(Guid.Parse(request.AccountId));

            string? retrieveRoleFromCurrentToken = _jwtHelper.ReturnRoleFromToken(existingAuth.LongLivedKey);

            RefreshLongLivedTokenReturn serverResponse = new RefreshLongLivedTokenReturn();

            if(existingAuth.Id == Guid.Empty || existingAuth.AccountId == Guid.Empty)
            {
                Log.Error("No valid auth has been previously setup, please ensure basic auth configuration has been carried out first");

                serverResponse.AccountId = request.AccountId;
                serverResponse.Success = false;
                serverResponse.RefreshedToken = "";

                return serverResponse;
            }

            string refreshedLongLivedToken = _tokenGeneratorService.GenerateLongLivedToken(existingAuth.AccountId.ToString(), retrieveRoleFromCurrentToken);

            StreamAuthUpdatesResponse mapToStreamUpdates = MapToStreamAuthUpdates(existingAuth.AccountId.ToString(), existingAuth.ShortLivedKey, refreshedLongLivedToken, UpdateType.LongLivedUpdate, existingAuth.Id.ToString());

            _transportationStorage.AddToStreamAuthUpdatesList(mapToStreamUpdates);

            AuthDataModel updatedAuthModel = await _authRepo.UpdateLongLivedToken(existingAuth, refreshedLongLivedToken);

            serverResponse.AccountId = updatedAuthModel.AccountId.ToString();
            serverResponse.Success = true;
            serverResponse.RefreshedToken = refreshedLongLivedToken;

            return serverResponse;
        }

        /// <summary>
        /// Refreshes the short lived key, as long as there's an already existing auth record for the account and the long lived key is still valid
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<RefreshShortLivedTokenReturn> RefreshShortLivedToken(RefreshShortLivedTokenDto request)
        {
            AuthDataModel existingAuth = await CheckForExistingAuth(Guid.Parse(request.AccountId));

            string? retrieveRoleFromCurrentToken = _jwtHelper.ReturnRoleFromToken(existingAuth.LongLivedKey);

            RefreshShortLivedTokenReturn serverResponse = new RefreshShortLivedTokenReturn();

            string? currentLongLivedKey = existingAuth.LongLivedKey;

            if (existingAuth.AccountId == Guid.Empty || existingAuth.LongLivedKey == string.Empty)
            {
                Log.Error($"No valid auth has been previously setup or the long lived key is currently invalid");

                serverResponse.AccountId = request.AccountId;
                serverResponse.Success = false;
                serverResponse.RefreshedToken = "";

                return serverResponse;
            }

            bool isLongKeyValid = _jwtHelper.IsLongLivedKeyValid(currentLongLivedKey);

            string refreshedShortLivedToken = _tokenGeneratorService.GenerateShortLivedToken(existingAuth.AccountId.ToString(), retrieveRoleFromCurrentToken);

            StreamAuthUpdatesResponse mapToStreamUpdates = MapToStreamAuthUpdates(existingAuth.AccountId.ToString(), refreshedShortLivedToken, currentLongLivedKey, UpdateType.ShortLivedUpdate, existingAuth.Id.ToString());

            _transportationStorage.AddToStreamAuthUpdatesList(mapToStreamUpdates);

            AuthDataModel authRecord = await _authRepo.UpdateShortLivedToken(existingAuth, refreshedShortLivedToken); 

            serverResponse.AccountId = existingAuth.AccountId.ToString();
            serverResponse.Success = true;
            serverResponse.RefreshedToken = refreshedShortLivedToken;

            return serverResponse;
        }

        /// <summary>
        /// Revokes a long lived token, this removes the long lived token and the short lived token that are connected to a users account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RevokeLongLivedTokenReturn> RevokeLongLivedToken(RevokeLongLivedTokenDto request)
        {
            AuthDataModel existingAuth = await _authRepo.CheckForExistingAuth(Guid.Parse(request.AccountId));

            RevokeLongLivedTokenReturn serverResponse = new RevokeLongLivedTokenReturn();

            if (existingAuth.AccountId == Guid.Empty || string.IsNullOrEmpty(existingAuth.LongLivedKey))
            {
                Log.Error($"No valid auth has previously been setup or registered"); 

                serverResponse.AccountId = request.AccountId;
                serverResponse.Success = false;

                return serverResponse;
            }

            AuthDataModel revokingKeys = await _authRepo.RevokeLongLivedToken(existingAuth);

            Log.Information($"{this.GetType().Namespace} Keys have been revoked for account with ID {existingAuth.AccountId}");

            serverResponse.AccountId = existingAuth.AccountId.ToString();
            serverResponse.Success = true;

            return serverResponse;
        }

        /// <summary>
        /// Incredibly simple login, checks for basic equality between the username and password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LoginReturn> Login(LoginDto request)
        {
            
            AccountDataModel retrievingAccount = await _authRepo.CheckForExistingAccountViaUsername(request.Username);

            LoginReturn serverResponse = new LoginReturn(); 

            if(retrievingAccount.Id == Guid.Empty)
            {
                Log.Warning($"No account exists with username {request.Username}");

                serverResponse.Username = request.Username;
                serverResponse.AccountId = "";
                serverResponse.Success = false;

                return serverResponse;
            }

            bool validatingUsernameMatch = ValidatingUsernameMatch(request.Username, retrievingAccount.Username);
            bool validatingPassMatch = ValidatingPasswordMatch(request.Password, retrievingAccount.Password);

            if (!validatingPassMatch || !validatingUsernameMatch)
            {
                serverResponse.Username = request.Username;
                serverResponse.AccountId = ""; 
                serverResponse.Success = false;

                return serverResponse;
            }

            serverResponse.Username = retrievingAccount.Username;
            serverResponse.AccountId = retrievingAccount.Id.ToString();
            serverResponse.Success = true;

            return serverResponse;



        }

        public async Task<ReinstateAuthKeyReturn> ReinstantiateAuthKey(ReinstateAuthKeyDto request)
        {

            AuthDataModel checkingForExistingKeys = await _authRepo.CheckForExistingAuth(Guid.Parse(request.AccountId));

            AccountDataModel? getRole = await _authRepo.RetrieveRoleFromAccount(checkingForExistingKeys.AccountId);

            ReinstateAuthKeyReturn serverResponse = new ReinstateAuthKeyReturn();

            if (checkingForExistingKeys.AccountId == Guid.Empty)
            {
                Log.Information($"No account can be found to re-instate keys for");

                serverResponse.AccountId = request.AccountId;
                serverResponse.ShortLivedKey = "";
                serverResponse.LongLivedKey = "";
                serverResponse.Success = false;

                return serverResponse;

            }

            string generateShortLivedKey = _tokenGeneratorService.GenerateShortLivedToken(checkingForExistingKeys.AccountId.ToString(), getRole.ToString());

            string generateLongLivedKey = _tokenGeneratorService.GenerateLongLivedToken(checkingForExistingKeys.Account.ToString(), getRole.ToString());

            await _authRepo.UpdateExistingAuthKeys(checkingForExistingKeys, generateLongLivedKey, generateShortLivedKey);

            serverResponse.AccountId = checkingForExistingKeys.AccountId.ToString();
            serverResponse.ShortLivedKey = generateShortLivedKey;
            serverResponse.LongLivedKey= generateLongLivedKey;
            serverResponse.Success = true;
         
            return serverResponse;
        }

        public async Task<SilentShortLivedTokenRefreshReturn> SilentTokenCycle(string longLivedToken)
        {

            SilentShortLivedTokenRefreshReturn serverResponse = new SilentShortLivedTokenRefreshReturn();

            string? accountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(longLivedToken);
            string? accountRoleFromToken = _jwtHelper.ReturnRoleFromToken(longLivedToken);

            if(accountIdFromToken == string.Empty || accountRoleFromToken == string.Empty)
            {
                serverResponse.RefreshedShortLivedToken = "";
                serverResponse.Success = false; 

                return serverResponse;
            }

            AuthDataModel authAccount = await _authRepo.CheckForExistingAuthViaAccountId(Guid.Parse(accountIdFromToken));

            string? refreshedShortLivedToken = _tokenGeneratorService.GenerateShortLivedToken(accountIdFromToken, accountRoleFromToken);

            StreamAuthUpdatesResponse mapToStreamUpdates = MapToStreamAuthUpdates(accountIdFromToken, refreshedShortLivedToken, longLivedToken, UpdateType.ShortLivedUpdate, authAccount.Id.ToString());

            _transportationStorage.AddToStreamAuthUpdatesList(mapToStreamUpdates);


            if (authAccount.Id == Guid.Empty || string.IsNullOrEmpty(authAccount.LongLivedKey))
            {
                Log.Information($"No account can be found with the provided account ID");

                serverResponse.RefreshedShortLivedToken = "";
                serverResponse.Success = false; 

                return serverResponse;
            }

            await _authRepo.UpdateShortLivedToken(authAccount, refreshedShortLivedToken);

            serverResponse.RefreshedShortLivedToken = refreshedShortLivedToken;
            serverResponse.Success = true;

            return serverResponse;



        }

        public override async Task StreamAuthCreations(StreamAuthCreationsRequest request, IServerStreamWriter<StreamAuthCreationsResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {

                var authCreation = _transportationStorage.ReturnStreamAuthCreationsList().ToImmutableList();

                if (authCreation.Count == 0)
                {
                    continue;
                }

                foreach (var item in authCreation)
                {
                    Log.Information($"Sending auth item with auth key {item.AuthKey}");

                    await responseStream.WriteAsync(item);
                }

                _transportationStorage.ClearAuthCreations();
            }

        }

        public override async Task StreamAuthKeyUpdates(StreamAuthUpdatesRequest request, IServerStreamWriter<StreamAuthUpdatesResponse> responseStream, ServerCallContext context)
        {

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var authUpdates = _transportationStorage.ReturnStreamAuthUpdatesList().ToImmutableList();

                if(authUpdates.Count == 0)
                {
                    continue;
                }

                foreach (var item in authUpdates)
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

                _transportationStorage.ClearAuthUpdates();
            }
            
        }

        private async Task<AccountDataModel> CheckForExistingAccount(Guid accountId)
        {
            AccountDataModel account = await _authRepo.CheckForExistingAccount(accountId);

            if(account.Id == Guid.Empty)
            {
                return new AccountDataModel();
            }

            return account;

        }

        private async Task<AuthDataModel> CheckForExistingAuth(Guid accountId)
        {
            AuthDataModel auth = await _authRepo.CheckForExistingAuth(accountId);

            if(auth.AccountId == Guid.Empty || auth.Id == Guid.Empty)
            {
                return new AuthDataModel();
            }

            return auth;
        }

        private AuthDataModel CreateAuthToAuthModel(CreateAuthDto createAuthRequest, AccountDataModel account, string shortLivedKey, string longLivedKey)
        {
            AuthDataModel authModel = new AuthDataModel
            {
                AccountId = Guid.Parse(createAuthRequest.AccountId),
                Account = account,
                Id = Guid.NewGuid(),
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
                AuthKey = authModel.Id.ToString(),
                AccountId = authModel.AccountId.ToString(),
                ShortLivedKey = authModel.ShortLivedKey,
                LongLivedKey = authModel.LongLivedKey, 
              
            };

            return newVaultResponse;
        }

        private StreamAuthUpdatesResponse MapToStreamAuthUpdates(string accountId, string? shortLivedKey, string? longLivedKey, UpdateType updateType, string authKey)
        {
            StreamAuthUpdatesResponse streamAuthUpdate = new StreamAuthUpdatesResponse
            {
                UpdateId = Guid.NewGuid().ToString(),
                AccountId = accountId,
                ShortLivedKey = shortLivedKey,
                LongLivedKey = longLivedKey,
                UpdateType = updateType,
                AuthKey = authKey
            };

            return streamAuthUpdate;
        }

        







    }
}
