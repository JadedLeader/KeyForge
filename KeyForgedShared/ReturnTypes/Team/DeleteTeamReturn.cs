using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Team
{
    public class DeleteTeamReturn
    {

        public string TeamId { get; set; }

        public string TeamName { get; set; }

        public bool Success { get; set; }

    }
}
