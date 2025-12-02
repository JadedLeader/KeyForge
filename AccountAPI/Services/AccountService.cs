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
using AccountAPI.DomainServices;
using AccountAPI.Interfaces.DomainServices;
using KeyForgedShared.ValidationType;

namespace AccountAPI.Services
{
    public class AccountService : Account.AccountBase, IAccountService
    {

        private readonly IAccountRepo _accountRepo;

        private readonly StreamStorage _streamStorage;

        private readonly IJwtHelper _jwtHelper;

        private readonly IAccountDomainService _accountDomain;

        public AccountService(IAccountRepo accountRepo, Channel<StreamAccountResponse> streamAccountResponseChannel, StreamStorage streamStorage, IJwtHelper jtwHelper, IAccountDomainService accountDomain)
        {
            _accountRepo = accountRepo;
            _streamStorage = streamStorage;
            _jwtHelper = jtwHelper;
            _accountDomain = accountDomain;
            
        }

        /// <summary>
        /// Creates an account within the system
        /// </summary>
        /// <param name="request">The request containing the details sent from the front end, typically containing user data</param>
        /// <param name="context">Server call context, containing any metadata</param>
        /// <returns>A create account response, returning the username added and the password</returns>
        public async Task<CreateAccountReturn> CreateAccount(CreateAccountDto request)
        {
            CreateAccountReturn createdAccount = new();

            if (!await _accountDomain.ValidateCreateAccount(request))
            {
                createdAccount.Success = false;

                return createdAccount;
            }

            string passwordEncrypted = EncryptPassword(request.Password);

            AccountDataModel mapToAccount = MapToAccount(request, passwordEncrypted);

            await _accountRepo.CreateAccount(mapToAccount);

            StreamAccountResponse addingToChannel = MapAccountModelToStream(mapToAccount);

            _streamStorage.AddToAccountCreationStream(addingToChannel);

            createdAccount.Username = request.Username;
            createdAccount.Password = passwordEncrypted;
            createdAccount.Success = true;

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

                await Task.Delay(250, context.CancellationToken);
            }
            
        }

        public async Task<DeleteAccountReturn> RemoveAccount(DeleteAccountDto request)
        {
            DeleteAccountReturn serverResponse = new DeleteAccountReturn();

            if (!await _accountDomain.ValidateDeleteAccount(request))
            {
                serverResponse.Success = false;

                return serverResponse;
            }

            _streamStorage.AddToAccountDeletionStream(Guid.Parse(request.AccountId));

            await _accountDomain.DeleteAccount(Guid.Parse(request.AccountId));

            serverResponse.AccountId = request.AccountId;
            serverResponse.Success = true;

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
            GetAccountDetailsReturn getAccountResponse = new();

            Guid accountIdFromToken = Guid.Parse(_jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            GetAccountValidationResult validated = await _accountDomain.ValidateGetAccountDetails(accountIdFromToken);

            if (!validated.IsValidated)
            {
                getAccountResponse.Success = false; 

                return getAccountResponse;
            }

            getAccountResponse.Username = validated.AccountModel.Username; 
            getAccountResponse.Email = validated.AccountModel.Email;
            getAccountResponse.AccountCreated = validated.AccountModel.AccountCreated.ToString();
            getAccountResponse.Success = true;

            return getAccountResponse;

        }

        public async Task<CheckPasswordMatchReturn> CheckPasswordMatch(PasswordMatchDto passwordMatchRequest, string shortLivedToken)
        {
            CheckPasswordMatchReturn checkPasswordMatchResponse = new();

            Guid accountIdFromToken = Guid.Parse( _jwtHelper.ReturnAccountIdFromToken(shortLivedToken));

            if (!await _accountDomain.ValidatePasswordMatch(passwordMatchRequest, accountIdFromToken))
            {
                checkPasswordMatchResponse.Success = false; 

                return checkPasswordMatchResponse;
            }

            checkPasswordMatchResponse.IsMatch = true;
            checkPasswordMatchResponse.Success = true;

            return checkPasswordMatchResponse;

        }

        private string EncryptPassword(string userPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(userPassword, 10);
        }

        private StreamAccountResponse MapAccountModelToStream(AccountDataModel model)
        {
            StreamAccountResponse response = new StreamAccountResponse
            {
                AccountId = model.Id.ToString(),
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                AuthRole = (AuthorisationRoles)model.AuthorisationLevel,
                AccountCreated = model.AccountCreated.ToString()

            };

            return response;
        }

        private AccountDataModel MapToAccount(CreateAccountDto createAccount, string encryptedPassword)
        {
            AccountDataModel newModel = new AccountDataModel
            {
                Id = Guid.NewGuid(),
                Username = createAccount.Username,
                Password = encryptedPassword,
                AccountCreated = DateTime.Now,
                AuthorisationLevel = AuthRoles.User,
                Email = createAccount.Email,
            };

            return newModel;
        }


        
    }
}
