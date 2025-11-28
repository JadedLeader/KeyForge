using KeyForgedShared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.SharedDataModels
{
    public class TeamDataModel : IEntity
    {

        [Key]
        [Column("TeamId")]
        public Guid Id { get; set; }

        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        public string TeamName { get; set; }

        public string TeamAcceptingInvites { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public int MemberCap { get; set; }

        public AccountDataModel Account { get; set; }

        public TeamVaultDataModel TeamVault { get; set; }

    }
}
