using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.VaultKeys
{
    public class UpdateVaultKeyAndKeyNameReturn
    {

        public string NewEncryptedKey { get; set; }

        public string NewKeyName { get; set; }

        public bool Success { get; set; }

        public string Description { get; set; }

    }
}
