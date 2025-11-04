using gRPCIntercommunicationService.Protos;
using KeyForgedShared.DTO_s.VaultDTO_s;
using KeyForgedShared.ReturnTypes.Vaults;
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
        public async Task<IActionResult> CreateVault([FromBody]CreateVaultDto createVaultRequest)
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                CreateVaultReturn createNewVault = await _vaultService.CreateVault(createVaultRequest, cookie);

                if(createNewVault.Sucessful == false)
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
        public async Task<IActionResult> DeleteVault([FromBody] DeleteVaultDto deleteVaultRequest)
        {
            bool shortLivedToken = Request.Cookies.TryGetValue("ShortLivedToken", out string? tokenCookie);

            bool vaultId = Request.Cookies.TryGetValue("VaultId", out string? vaultIdCookie);

            if(shortLivedToken && vaultId)
            {
                DeleteVaultReturn deleteVault = await _vaultService.DeleteVault(deleteVaultRequest, tokenCookie, vaultIdCookie);

                if(deleteVault.Sucessful == false)
                {
                    return BadRequest(deleteVault);
                }

                return Ok(deleteVault);
            }
            
            return NotFound();

            
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVault([FromBody] UpdateVaultNameDto updateVaultNameRequest)
        {
            bool shortLivedToken = Request.Cookies.TryGetValue("ShortLivedToken", out string? shortLivedTokenCookie);

            bool vaultId = Request.Cookies.TryGetValue("VaultId", out string? vaultIdCookie);

            if(shortLivedToken && vaultId)
            {
                UpdateVaultNameReturn updateVaultName = await _vaultService.UpdateVaultName(updateVaultNameRequest, shortLivedTokenCookie, vaultIdCookie);

                if(updateVaultName.Sucessful == false)
                {
                    return BadRequest(updateVaultName); 
                }

                return Ok(updateVaultName);
            }

            return Unauthorized();
        }

    }
}
