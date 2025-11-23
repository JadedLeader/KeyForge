using KeyForgedShared.SharedDataModels;
using AccountAPI.Interfaces.RepoInterface;
using AccountAPI.Interfaces.ServiceInterface;
using Grpc.Core;
using gRPCIntercommunicationService;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Channels;
using Serilog;
using AccountAPI.Storage;
using KeyForgedShared.Interfaces;
using KeyForgedShared.ReturnTypes.Accounts;
using System.Data.SqlTypes;
using KeyForgedShared.DTO_s.AccountDTO_s;
using System.Collections.Immutable;

namespace AccountAPI.Services
{
    public class AccountService : Account.AccountBase, IAccountService
    {

        private readonly IAccountRepo _accountRepo;

        private readonly StreamStorage _streamStorage;

        private readonly IJwtHelper _jwtHelper;

        public AccountService(IAccountRepo accountRepo, Channel<StreamAccountResponse> streamAccountResponseChannel, StreamStorage streamStorage, IJwtHelper jtwHelper)
        {
            _accountRepo = accountRepo;
            _streamStorage = streamStorage;
            _jwtHelper = jtwHelper;
            
        }

        /// <summary>
        /// Creates an account within the system
        /// </summary>
        /// <param name="request">The request containing the details sent from the front end, typically containing user data</param>
        /// <param name="context">Server call context, containing any metadata</param>
        /// <returns>A create account response, returning the username added and the password</returns>
        public async Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request)
        {

            AccountDataModel mappingRequestToAccount = MapRequestToAccount(request);

            CreateAccountResponse createdAccount = new CreateAccountResponse();

            Log.Information($"ENDPOINT HIT");

            if(mappingRequestToAccount.Username == string.Empty || request.Password == string.Empty)
            {
                createdAccount.Username = "";
                createdAccount.Password = "";
                createdAccount.Successful = false;
            }

            string passwordEncrypted = EncryptPassword(request.Password);

            mappingRequestToAccount.Password = passwordEncrypted;

            await _accountRepo.CreateAccount(mappingRequestToAccount);

            createdAccount.Username = mappingRequestToAccount.Username;
            createdAccount.Password = passwordEncrypted;
            createdAccount.Successful = true;

            StreamAccountResponse addingToChannel = MapAccountModelToStream(mappingRequestToAccount);

            _streamStorage.AddToAccountCreationStream(addingToChannel);
            int length = _streamStorage.AccountCreationStreamTotal();

            return createdAccount;

        }

        public override async Task StreamAccount(StreamAccountRequest request, IServerStreamWriter<StreamAccountResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {

                var getAccountStreams = _streamStorage.ReturnAccountCreationStream().ToImmutableList();

                if(getAccountStreams.Count == 0)
                {
                    continue;
                }

                foreach (StreamAccountResponse item in getAccountStreams)
                {
                    Log.Information($"streaming requets {item} to auth");

                    await responseStream.WriteAsync(item);
                }

                _streamStorage.ClearAccountCreationStream();
            }
            
        }

        public async Task<DeleteAccountResponse> RemoveAccount(DeleteAccountRequest request)
        {
            AccountDataModel checkForExistingAccount = await _accountRepo.CheckForExistingAccount(Guid.Parse(request.AccountId));

            DeleteAccountResponse serverResponse = new DeleteAccountResponse();

            if (checkForExistingAccount.AccountId == Guid.Empty)
            {
                Log.Error($"No Account with ID {request.AccountId} could be found"); 

                serverResponse.AccountId = request.AccountId;
                serverResponse.Username = "";
                serverResponse.Successful = false; 
                
                return serverResponse;
            }

            _streamStorage.AddToAccountDeletionStream(checkForExistingAccount.AccountId);

            await _accountRepo.DeleteAccount(checkForExistingAccount);

            serverResponse.AccountId = checkForExistingAccount.AccountId.ToString();
            serverResponse.Username = checkForExistingAccount.Username;
            serverResponse.Successful = true;

            return serverResponse;
        }


        public override async Task StreamAccountDeletions(StreamAccountDeleteRequest request, IServerStreamWriter<StreamAccountDeleteResponse> responseStream, ServerCallContext context)
        {

            while (!context.CancellationToken.IsCancellationRequested)
            {

                var getAccountDeletions = _streamStorage.ReturnAccountDeletionList().ToImmutableList();

                if(getAccountDeletions.Count == 0)
                {
                    continue;
                }

                foreach (Guid accountDeletionId in getAccountDeletions)
                {
                    StreamAccountDeleteResponse newDeleteResponse = new StreamAccountDeleteResponse
                    {
                        AccountId = accountDeletionId.ToString(),
                    };

                    Log.Information($"Sending deletion request for account with ID {newDeleteResponse.AccountId}");

                    await responseStream.WriteAsync(newDeleteResponse);
                }

                _streamStorage.ClearAccountDeletionsStream();
            }
            
        }

        public async Task<GetAccountDetailsReturn> GetAccountDetails(string shortLivedToken)
        {

            GetAccountDetailsReturn getAccountResponse = new GetAccountDetailsReturn();

            string? accountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(accountIdFromToken == string.Empty)
            {
                getAccountResponse.Username = "";
                getAccountResponse.AccountCreated = "";
                getAccountResponse.Email = "";
                getAccountResponse.Success = false;

                return getAccountResponse;
            }

            GetAccountDetailsReturn getAccountDetails = await _accountRepo.GetUserAccount(Guid.Parse(accountIdFromToken));

            if(getAccountDetails == null)
            {
                getAccountResponse.Username = "";
                getAccountResponse.AccountCreated = "";
                getAccountResponse.Email = "";
                getAccountResponse.Success = false;

                return getAccountResponse;
            }

            getAccountResponse.Username = getAccountDetails.Username; 
            getAccountResponse.Email = getAccountDetails.Email;
            getAccountResponse.AccountCreated = getAccountDetails.AccountCreated;
            getAccountResponse.Success = true;

            return getAccountResponse;

        }

        public async Task<CheckPasswordMatchReturn> CheckPasswordMatch(PasswordMatchDto passwordMatchRequest, string shortLivedToken)
        {
            CheckPasswordMatchReturn checkPasswordMatchResponse = new CheckPasswordMatchReturn();

            string? accountIdFromToken = _jwtHelper.ReturnAccountIdFromToken(shortLivedToken);

            if(accountIdFromToken == string.Empty)
            {
                checkPasswordMatchResponse.IsMatch = false;
                checkPasswordMatchResponse.IsMatch = false;

                return checkPasswordMatchResponse;
            }

            string? hashedPassword = await _accountRepo.GetHashedPassword(Guid.Parse(accountIdFromToken));

            if(hashedPassword == string.Empty)
            {
                checkPasswordMatchResponse.IsMatch = false;
                checkPasswordMatchResponse.IsMatch = false;

                return checkPasswordMatchResponse;
            }

            bool isPasswordMatch = CheckPasswordMatchesHash(passwordMatchRequest.Password, hashedPassword);

            if (!isPasswordMatch)
            {
                checkPasswordMatchResponse.IsMatch = false;
                checkPasswordMatchResponse.IsMatch = false;

                return checkPasswordMatchResponse;
            }

            checkPasswordMatchResponse.IsMatch = true;
            checkPasswordMatchResponse.Success = true;

            return checkPasswordMatchResponse;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        private string EncryptPassword(string userPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(userPassword, 10);
        }

        private bool CheckPasswordMatchesHash(string passwordToCheck, string hashToCheck)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(passwordToCheck, hashToCheck);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createAccountRequest"></param>
        /// <returns></returns>
        private AccountDataModel MapRequestToAccount(CreateAccountRequest createAccountRequest)
        {
            AccountDataModel newModel = new AccountDataModel
            {
                AccountId = Guid.NewGuid(),
                Username = createAccountRequest.Username,
                Password = createAccountRequest.Password,
                AccountCreated = DateTime.Now,
                AuthorisationLevel = AuthRoles.User,
                Email = createAccountRequest.Email,
            }; 

            return newModel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private StreamAccountResponse MapAccountModelToStream(AccountDataModel model)
        {
            StreamAccountResponse response = new StreamAccountResponse
            {
                AccountId = model.AccountId.ToString(),
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                AuthRole = (AuthorisationRoles)model.AuthorisationLevel,
                AccountCreated = model.AccountCreated.ToString()

            };

            return response;
        }


        
    }
}
