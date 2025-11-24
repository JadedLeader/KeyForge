using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Team
{
    public class CreateTeamReturn
    {

        public string TeamId { get; set; }

        public string TeamName { get; set; }

        public string TeamAcceptingInvites { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedAt { get; set; }

        public int MemberCap { get; set; }

        public bool Success { get; set; }
    }
}
