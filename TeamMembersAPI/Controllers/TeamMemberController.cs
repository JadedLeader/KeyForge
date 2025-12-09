using KeyForgedShared.DTO_s.TeamMemberDTO_s;
using KeyForgedShared.ReturnTypes.TeamMember;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using TeamMembersAPI.Interfaces.Services;

namespace TeamMembersAPI.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class TeamMemberController : ControllerBase
    {

        private readonly ITeamMembersService _teamMembersService;

        public TeamMemberController(ITeamMembersService teamMembersService)
        {
            _teamMembersService = teamMembersService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamMember([FromBody] CreateTeamMemberDto createTeamMember)
        {
            if(!Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                return Unauthorized(); 
            }

            CreateTeamMemberReturn teamMember = await _teamMembersService.CreateTeamMember(createTeamMember, cookie);

            if (!teamMember.Success)
            {
                return BadRequest(teamMember);
            }

            return Ok(teamMember);
        }
    }
}
