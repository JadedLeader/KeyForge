using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.DTO_s.VaultKeysDTO_s
{
    public class VaultKeyDto
    {

        public Guid VaultKeyId { get; set; }
        public string KeyName { get; set; }
        public string HashedVaultKey { get; set; }
        public string DateTimeVaultKeyCreated { get; set; }

    }
}
