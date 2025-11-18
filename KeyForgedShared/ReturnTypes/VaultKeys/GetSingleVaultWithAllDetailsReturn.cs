using KeyForgedShared.ReturnTypes.Vaults;
using KeyForgedShared.SharedDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.VaultKeys
{
    public class GetSingleVaultWithAllDetailsReturn
    {

        public string VaultId { get; set; }

        public string AccountId { get; set; }

        public string VaultName { get; set; }

        public string VaultCreatedAt { get; set; }

        public KeyForgedShared.SharedDataModels.VaultType VaultType { get;set; }

        public bool Success { get; set; }

        public List<VaultKeysDataModel> VaultKeys { get; set; } = new List<VaultKeysDataModel>();

    }
}
