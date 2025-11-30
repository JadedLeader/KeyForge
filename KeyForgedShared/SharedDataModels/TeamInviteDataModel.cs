using KeyForgedShared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.SharedDataModels
{
    public class TeamInviteDataModel : IEntity
    {

        [Key]
        [Column("TeamInviteId")]
        public Guid Id { get; set; }

        [ForeignKey("AccountId")]

        public Guid AccountId { get; set; }

        [ForeignKey("TeamVaultId")]
        public Guid TeamVaultId { get; set; }

        public string InviteSentBy { get; set; }

        public string InviteRecipient { get; set; }

        public string InviteStatus { get; set; }

        public string InviteCreatedAt { get; set; }

        public AccountDataModel Account { get; set; }   

        public TeamVaultDataModel TeamVault { get; set; }
    }
}
