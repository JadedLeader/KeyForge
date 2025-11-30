using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.TeamInvite
{
    public class CreateTeamInviteReturn
    {

        public string InviteRecipient { get; set; }

        public bool Success { get; set; }

        public string InviteCreatedAt { get; set; }

    }
}
