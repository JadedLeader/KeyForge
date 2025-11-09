using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.VaultKeys
{
    public class AddVaultKeyReturn
    {

        public string VaultName { get; set; }   

        public string KeyName { get; set; }

        public string HashedKey { get; set; }

        public bool Success { get; set; }

        public string TimeOfKeyCreation { get; set; }

    }
}
