using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.DTO_s.VaultKeysDTO_s
{
    public class UpdateVaultKeyDto
    {

        public string VaultId { get; set; }

        public string? ChangedKeyName { get; set; } 

        public string? ChangedEncryptedVaultKey { get; set; }

    }
}
