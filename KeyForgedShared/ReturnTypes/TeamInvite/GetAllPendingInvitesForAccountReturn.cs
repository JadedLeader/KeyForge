using KeyForgedShared.Projections.TeamInviteProjections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.TeamInvite
{
    public class GetAllPendingInvitesForAccountReturn
    {

        public List<PendingTeamInvitesProjection> PendingTeamInvites = new();

        public bool Success { get; set; }

    }
}
