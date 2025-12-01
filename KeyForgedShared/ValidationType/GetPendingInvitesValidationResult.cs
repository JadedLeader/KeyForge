using KeyForgedShared.Projections.TeamInviteProjections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ValidationType
{
    public class GetPendingInvitesValidationResult
    {


        public List<PendingTeamInvitesProjection> PendingTeamInvites = new();
        public bool PendingInvitesSuccess { get;set; }

    }
}
