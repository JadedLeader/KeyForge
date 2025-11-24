using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.DTO_s.TeamDTO_s
{
    public class UpdateTeamDto
    {

        public Guid TeamId { get; set; }
        public string NewTeamName { get; set; }

        public string TeamAcceptingInvites { get; set; }

        public int MemberCap { get; set; }

    }
}
