using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.VaultKeys
{
    public class RemoveAllVaultKeysReturn
    {

        public string VaultId { get;set; }


        public List<Guid> KeysDeleted = new List<Guid>();

        public bool Success { get; set; }   

    }
}
