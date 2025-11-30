using KeyForgedShared.DTO_s.TeamInviteDTO_s;
using KeyForgedShared.ReturnTypes.TeamInvite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamInviteAPI.Interfaces.Services;

namespace TeamInviteAPI.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class TeamInviteController : ControllerBase
    {

        private readonly ITeamInviteService _teamInviteService;

        public TeamInviteController(ITeamInviteService teamInviteService)
        {
            _teamInviteService = teamInviteService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamInvite([FromBody] CreateTeamInviteDto createTeamInvite)
        {
            if(!Request.Cookies.TryGetValue($"ShortLivedToken", out string? cookie))
            {
                return Unauthorized();
            }

            CreateTeamInviteReturn createdTeamInvite = await _teamInviteService.CreateTeamInvite(createTeamInvite, cookie);

            if (!createdTeamInvite.Success)
            {
                return BadRequest(createdTeamInvite);
            }

            return Ok(createdTeamInvite);
        }

    }
}
