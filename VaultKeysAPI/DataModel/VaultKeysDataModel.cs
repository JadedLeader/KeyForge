using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaultKeysAPI.DataModel
{
    public class VaultKeysDataModel
    {

        [Key]
        public Guid VaultKeyId { get; set; }

        [ForeignKey("VaultId")]
        public Guid VaultId { get; set; } 

        public string HashedVaultKey { get; set; }

        public VaultDataModel Vault { get; set; }

    }
}
