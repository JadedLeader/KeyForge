using KeyForgedShared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyForgedShared.SharedDataModels
{
    public class VaultKeysDataModel: IEntity
    {

        [Key]
        public Guid VaultKeyId { get; set; }

        public Guid Id => VaultKeyId;

        [ForeignKey("VaultId")]
        public Guid VaultId { get; set; } 

        public string KeyName { get; set; }

        public string HashedVaultKey { get; set; }

        public VaultDataModel Vault { get; set; }

        public string DateTimeVaultKeyCreated { get; set; }

    }
}
