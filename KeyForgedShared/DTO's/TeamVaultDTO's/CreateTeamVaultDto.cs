using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.DTO_s.TeamVaultDTO_s
{
    public class CreateTeamVaultDto
    {

        public string TeamId { get; set; }

        public string TeamVaultDescription { get; set; }

        public string TeamVaultName { get; set; }

        public string CurrentStatus { get; set; }

    }
}
