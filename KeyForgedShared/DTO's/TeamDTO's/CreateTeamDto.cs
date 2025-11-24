using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.DTO_s.TeamDTO_s
{
    public class CreateTeamDto
    {

        public string TeamName { get; set; }

        public string TeamAcceptingInvites { get; set; }

        public int MemberCap { get; set; }

    }
}
