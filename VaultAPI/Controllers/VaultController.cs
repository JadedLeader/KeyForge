using gRPCIntercommunicationService.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.VisualBasic;
using Serilog;
using VaultAPI.Interfaces.ServiceInterfaces;

namespace VaultAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class VaultController : ControllerBase
    {

        private readonly IVaultService _vaultService;

        public VaultController(IVaultService vaultService)
        {
            _vaultService = vaultService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVault([FromBody]CreateVaultRequest createVaultRequest)
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                CreateVaultResponse createNewVault = await _vaultService.CreateVault(createVaultRequest, cookie);

                if(createNewVault.Sucessfull == false)
                {
                    Log.Warning($"{this.GetType().Namespace} An error occurred when creating a vault");

                    return BadRequest(createNewVault);
                }

                Response.Cookies.Append("VaultId", createNewVault.VaultId);
                Response.Cookies.Append("TypeOfVault", createNewVault.VaultType.ToString());

                return Ok(createNewVault);
            }

            return Unauthorized();
           
        }

        [HttpDelete] 
        public async Task<IActionResult> DeleteVault([FromBody] DeleteVaultRequest deleteVaultRequest)
        {
            bool shortLivedToken = Request.Cookies.TryGetValue("ShortLivedToken", out string? tokenCookie);

            bool vaultId = Request.Cookies.TryGetValue("VaultId", out string? vaultIdCookie);

            if(shortLivedToken && vaultId)
            {
                DeleteVaultResponse deleteVault = await _vaultService.DeleteVault(deleteVaultRequest, tokenCookie, vaultIdCookie);

                if(deleteVault.Successfull == false)
                {
                    return BadRequest(deleteVault);
                }

                return Ok(deleteVault);
            }
            
            return NotFound();

            
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVault([FromBody] UpdateVaultNameRequest updateVaultNameRequest)
        {
            bool shortLivedToken = Request.Cookies.TryGetValue("ShortLivedToken", out string? shortLivedTokenCookie);

            bool vaultId = Request.Cookies.TryGetValue("VaultId", out string? vaultIdCookie);

            if(shortLivedToken && vaultId)
            {
                UpdateVaultNameResponse updateVaultName = await _vaultService.UpdateVaultName(updateVaultNameRequest, shortLivedTokenCookie, vaultIdCookie);

                if(updateVaultName.Successfull == false)
                {
                    return BadRequest(updateVaultName); 
                }

                return Ok(updateVaultName);
            }

            return Unauthorized();
        }



    }
}
