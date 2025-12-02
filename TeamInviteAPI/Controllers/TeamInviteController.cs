using KeyForgedShared.DTO_s.TeamInviteDTO_s;
using KeyForgedShared.ReturnTypes.TeamInvite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
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

        [HttpPost]
        public async Task<IActionResult> GetPendingTeamInvites([FromBody] GetCurrentPendingTeamInvitesDto pendingTeamInvites)
        {
            if(!Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                return Unauthorized();
            }

            GetCurrentPendingTeamInvitesReturn returnedPendingInvites = await _teamInviteService.GetCurrentPendingTeamInvites(pendingTeamInvites, cookie);

            if (!returnedPendingInvites.Success)
            {
                return BadRequest(returnedPendingInvites);
            }

            return Ok(returnedPendingInvites);
        }

        [HttpDelete]
        public async Task<IActionResult> RejectTeamInvite([FromBody] RejectTeamInviteDto rejectTeamInvite)
        {
            if(!Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                return Unauthorized();
            }

            RejectTeamInviteReturn rejectedTeaminvite = await _teamInviteService.RejectTeamInvite(rejectTeamInvite, cookie);

            if (!rejectedTeaminvite.Success)
            {
                return BadRequest(rejectedTeaminvite);
            }

            return Ok(rejectedTeaminvite); 
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTeamInvite([FromBody] UpdateTeamInviteDto updateTeamInvite)
        {
            if(!Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                return Unauthorized();
            }

            UpdateTeamInviteReturn updatedTeamInvite = await _teamInviteService.UpdateTeamInvite(updateTeamInvite, cookie);

            if (!updatedTeamInvite.Success)
            {
                return BadRequest(updatedTeamInvite);
            }

            return Ok(updatedTeamInvite);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllPendingInvitesForAccount([FromBody] GetAllPendingInvitesForAccountDto pendingInvites)
        {
            if(!Request.Cookies.TryGetValue("ShortLivedToken", out string? cookie))
            {
                return Unauthorized();
            }

            GetAllPendingInvitesForAccountReturn getPendingInvites = await _teamInviteService.GetAllPendingInvitesForAccount(pendingInvites, cookie);

            if (!getPendingInvites.Success)
            {
                return BadRequest(getPendingInvites);
            }

            return Ok(getPendingInvites);
        }


    }
}
