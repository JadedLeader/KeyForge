using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Vaults
{
    public class UpdateVaultNameReturn
    {

        public string VaultId { get; set; } 

        public string UpdatedVaultName { get; set; }

        public bool Sucessful { get; set; }

    }
}
