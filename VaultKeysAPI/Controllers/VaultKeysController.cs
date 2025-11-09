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
    }
}
