using KeyForgedShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.Projections.TeamInviteProjections
{
    public class PendingTeamInvitesProjection
    {

        public string TeamInviteId { get; set; }
        public string InviteSentBy { get; set; }

        public string InviteRecipient { get; set; }

        public string InviteCreatedAt { get; set; }

    }
}
