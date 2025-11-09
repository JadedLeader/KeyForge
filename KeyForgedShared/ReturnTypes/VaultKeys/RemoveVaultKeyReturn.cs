using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.VaultKeys
{
    public class RemoveVaultKeyReturn
    {

        public string VaultName { get; set; }

        public string KeyName { get; set; }

        public bool Success { get; set; }    

    }
}
