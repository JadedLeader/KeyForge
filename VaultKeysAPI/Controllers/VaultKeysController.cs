using KeyForgedShared.DTO_s.VaultKeysDTO_s;
using KeyForgedShared.ReturnTypes.VaultKeys;
using Microsoft.AspNetCore.Mvc;
using VaultKeysAPI.Interfaces;

namespace VaultKeysAPI.Controllers
{

    [ApiController]
    [Route("[controller]/[Action]")]
    public class VaultKeysController : Controller
    {

        private readonly IVaultKeysService _vaultKeysService;
        public VaultKeysController(IVaultKeysService vaultKeysService)
        {
            _vaultKeysService = vaultKeysService;
        }

        [HttpPost]

        public async Task<IActionResult> CreateVaultkey([FromBody] AddVaultKeyDto addVaultKey)
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? shortLivedToken))
            {

                AddVaultKeyReturn vaultKey = await _vaultKeysService.AddVaultKey(addVaultKey, shortLivedToken);

                if (!vaultKey.Success)
                {
                    return BadRequest();
                }

                return Ok(vaultKey);
                
            }

            return Unauthorized();
        }


        [HttpPost]
        public async Task<IActionResult> DecryptVaultKey([FromBody] DecryptVaultKeyDto decryptVaultKey)
        {
            if(Request.Cookies.TryGetValue($"ShortLivedToken", out string? cookie))
            {
                DecryptVaultKeyReturn decryptedVaultKey = await _vaultKeysService.DecryptVaultKey(decryptVaultKey, cookie);

                if (!decryptedVaultKey.Sucess)
                {
                    return BadRequest();
                }

                return Ok(decryptedVaultKey);
            }

            return Unauthorized();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveVaultkey([FromBody] RemoveVaultKeyDto removeVaultKey)
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                RemoveVaultKeyReturn removedVaultKey = await _vaultKeysService.RemoveVaultKey(removeVaultKey, cookie);

                if (!removedVaultKey.Success)
                {
                    return BadRequest();
                }

                return Ok(removedVaultKey);
            }

            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVaultsWithKeys()
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                List<GetAllVaultsDto> getAllVaults = await _vaultKeysService.ReturnAllVaultsForUser(cookie);

                if(getAllVaults.Count >= 0)
                {
                    return Ok(getAllVaults);
                }

                return BadRequest();
            }

            return Unauthorized();
        }
    }
}
