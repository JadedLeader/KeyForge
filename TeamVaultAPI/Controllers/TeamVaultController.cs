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

        [HttpGet]
        public async Task<IActionResult> GetTeamsWithNoVaults()
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                GetTeamWithNoVaultsReturn? getTeam = await _teamVaultService.GetTeamsWithNoVaults(cookie);

                if (!getTeam.Success)
                {
                    return BadRequest(getTeam);
                }

                return Ok(getTeam);
            }

            return Unauthorized();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTeamVault([FromBody] DeleteTeamVaultDto deleteTeamVault)
        {
            if(Request.Cookies.TryGetValue($"ShortLivedToken", out string? cookie))
            {
                DeleteTeamVaultReturn deletedTeamVault = await _teamVaultService.DeleteTeamVault(deleteTeamVault, cookie);

                if (!deletedTeamVault.Success)
                {
                    return BadRequest(deletedTeamVault);
                }
                
                return Ok(deletedTeamVault);
            }

            return Unauthorized();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTeamVault([FromBody] UpdateTeamVaultDto updateTeamVault)
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                UpdateTeamVaultReturn updatedTeamVault = await _teamVaultService.UpdateTeamVault(updateTeamVault, cookie);

                if (!updatedTeamVault.Success)
                {
                    return BadRequest(updatedTeamVault);
                }

                return Ok(updatedTeamVault);
            }

            return Unauthorized();
        }

    }
}
