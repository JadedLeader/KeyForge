using AccountAPI.Interfaces.ServiceInterface;
using Grpc.Core;
using gRPCIntercommunicationService;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> CreateAccounts([FromBody] CreateAccountRequest request)
        {
            CreateAccountResponse serviceResponse =
                await _accountService.CreateAccount(request);

            if (serviceResponse.Successful == false)
            {
                return StatusCode(500);
            }

            return Ok(serviceResponse);

            
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            throw new NotImplementedException();
        }
    }
}
