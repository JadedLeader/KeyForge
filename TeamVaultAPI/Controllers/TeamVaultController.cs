using KeyForgedShared.DTO_s.TeamVaultDTO_s;
using KeyForgedShared.ReturnTypes.TeamVault;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TeamVaultAPI.Interfaces.Services;

namespace TeamVaultAPI.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class TeamVaultController : ControllerBase
    {

        private readonly ITeamVaultService _teamVaultService;

        public TeamVaultController(ITeamVaultService teamVaultService)
        {
            _teamVaultService = teamVaultService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamVault([FromBody] CreateTeamVaultDto createTeamVault)
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                CreateTeamVaultReturn createdTeam = await _teamVaultService.CreateTeamVault(createTeamVault, cookie);

                if (!createdTeam.Success)
                {
                    return BadRequest(createdTeam);
                }

                return Ok(createdTeam);
            }

            return Unauthorized();
        }

    }
}
