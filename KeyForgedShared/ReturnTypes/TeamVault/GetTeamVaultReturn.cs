using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.TeamVault
{
    public class GetTeamVaultReturn
    {

        public string TeamVaultName { get; set; }

        public string TeamVaultDescription { get; set; }

        public string CurrentStatus { get; set; }

        public bool Success { get; set; }

    }
}
