using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.SharedDataModels
{
    public class TeamVaultDataModel
    {

        [Key]
        public Guid TeamVaultId { get; set; }

        [ForeignKey("TeamId")]
        public Guid TeamId { get; set; }

        public string TeamVaultDescription { get; set; }

        public string TeamVaultName { get; set; }

        public string CreatedAt { get; set; }

        public string LastTeamUpdate { get; set; }

        public string CurrentStatus { get; set; }

        public TeamDataModel Team { get; set; }


    }
}
