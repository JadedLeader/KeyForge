using KeyForgedShared.DTO_s.TeamDTO_s;
using KeyForgedShared.ReturnTypes.Team;
using KeyForgedShared.ReturnTypes.Vaults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using TeamAPI.Interfaces.Services;

namespace TeamAPI.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto createTeam)
        {
        
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                CreateTeamReturn createdTeam = await _teamService.CreateTeam(createTeam, cookie);

                if (!createdTeam.Success)
                {
                    return BadRequest(createdTeam);
                }

                return Ok(createdTeam);
            }

            return Unauthorized();
        
        }

        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            if(Request.Cookies.TryGetValue($"ShortLivedToken", out string? cookie))
            {
                GetTeamsReturn teams = await _teamService.GetTeamsForAccount(cookie);

                if (!teams.Success)
                {
                    return BadRequest(teams);
                }

                return Ok(teams);
            }

            return Unauthorized();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTeam([FromBody] DeleteTeamDto deleteTeam)
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                DeleteTeamReturn deletedVault = await _teamService.DeleteTeam(deleteTeam, cookie);

                if (!deletedVault.Success)
                {
                    return BadRequest(deletedVault);
                }

                return Ok(deletedVault);
            }

            return Unauthorized();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTeam([FromBody] UpdateTeamDto updateTeam)
        {
            if(Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                UpdateTeamReturn updatedTeam = await _teamService.UpdateTeam(updateTeam, cookie);

                if (!updatedTeam.Success)
                {
                    return BadRequest(updatedTeam);
                }

                return Ok(updatedTeam);
            }

            return Unauthorized();
        }

    }
}
