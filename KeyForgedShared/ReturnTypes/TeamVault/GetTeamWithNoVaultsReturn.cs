using KeyForgedShared.Projections.TeamVaultProjections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.TeamVault
{
    public class GetTeamWithNoVaultsReturn
    {

        public List<GetTeamWithNoVault> TeamsWithNoVaults { get; set; }

        public bool Success { get; set; }

    }
}
