using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.SharedDataModels
{
    public class TeamMemberDataModel
    {

        [Key]
        [Column("TeamMemberId")]
        public Guid Id { get; set; }

        [ForeignKey("TeamVaultId")]
        public Guid TeamVaultId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string RoleName { get; set; }

        public string RoleAccess { get; set; }

        public TeamVaultDataModel TeamVault { get; set; }

    }
}
