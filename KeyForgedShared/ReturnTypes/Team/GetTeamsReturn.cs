using KeyForgedShared.SharedDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Team
{
    public class GetTeamsReturn
    {

        public List<TeamDataModel> Teams { get; set; }

        public bool Success { get; set; }

    }
}
