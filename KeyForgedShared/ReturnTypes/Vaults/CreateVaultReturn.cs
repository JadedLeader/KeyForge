using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Vaults
{
    public class CreateVaultReturn
    {

        public string AccountId { get; set; }

        public string VaultId { get; set; }

        public string VaultName { get; set; }

        public VaultType VaultType { get ; set; }   
        public bool Sucessful { get; set; }

    }

    public enum VaultType
    {
        Personal, 
        Team
    }
}
