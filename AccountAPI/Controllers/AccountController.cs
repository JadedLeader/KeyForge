using AccountAPI.Interfaces.ServiceInterface;
using Grpc.Core;
using gRPCIntercommunicationService;
using KeyForgedShared.DTO_s.AccountDTO_s;
using KeyForgedShared.ReturnTypes.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AccountAPI.Controllers
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateAccounts([FromBody] CreateAccountDto request)
        {
            CreateAccountReturn serviceResponse = await _accountService.CreateAccount(request);

            if (!serviceResponse.Success)
            {
                return BadRequest(serviceResponse);
            }

            return Ok(serviceResponse);

            
        }

        [AllowAnonymous]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto deleteAccountRequest)
        {
            DeleteAccountReturn deleteAccount = await _accountService.RemoveAccount(deleteAccountRequest);

            if (!deleteAccount.Success)
            {
                Log.Error($"{this.GetType().Namespace} An error occurred when trying to delete an account");

                return BadRequest(deleteAccount); 
            }

            return Ok(deleteAccount);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAccountDetails()
        {
            if(Request.Cookies.TryGetValue("LongLivedToken", out string? cookie))
            {
                GetAccountDetailsReturn details = await _accountService.GetAccountDetails(cookie);

                if (!details.Success)
                {
                    return BadRequest();
                }

                return Ok(details);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CheckPasswordIsValid([FromBody] PasswordMatchDto passwordMatchRequest)
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                CheckPasswordMatchReturn checkPasswordMatch = await _accountService.CheckPasswordMatch(passwordMatchRequest, cookie);

                if (!checkPasswordMatch.Success)
                {
                    return BadRequest();
                }

                return Ok(checkPasswordMatch);
            }

            return BadRequest();
        }
    }
}
