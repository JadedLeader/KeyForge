using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Team
{
    public class UpdateTeamReturn
    {

        public string TeamName { get; set; }

        public string TeamAcceptingInvites { get; set; }

        public int MemberCap { get; set; }

        public bool Success { get; set; }

    }
}
