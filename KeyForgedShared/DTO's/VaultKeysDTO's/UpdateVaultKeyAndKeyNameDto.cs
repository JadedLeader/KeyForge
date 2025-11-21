using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.DTO_s.VaultKeysDTO_s
{
    public class UpdateVaultKeyAndKeyNameDto
    {

        public string VaultKeyId { get; set; }

        public string? NewKeyName { get; set; }

        public string? NewVaultKey { get; set; }

    }
}
