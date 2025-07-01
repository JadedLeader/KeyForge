using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VaultAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class VaultController : ControllerBase
    {
        public VaultController()
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateVault()
        {
            throw new NotImplementedException();
        }

        [HttpDelete] 
        public async Task<IActionResult> DeleteVault()
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVault()
        {
            throw new NotImplementedException();
        }



    }
}
