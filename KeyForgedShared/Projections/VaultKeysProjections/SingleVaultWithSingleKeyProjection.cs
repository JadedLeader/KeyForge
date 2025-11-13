using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.Projections.VaultKeysProjections
{
    public class SingleVaultWithSingleKeyProjection
    {

        public string VaultName { get; set; }

        public SingleVaultKeyProjection singleVaultKey { get; set; }

    }
}
