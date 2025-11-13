using KeyForgedShared.ReturnTypes.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.DTO_s.VaultKeysDTO_s
{
    public class GetAllVaultsDto
    {

        public Guid VaultId { get; set; }
        public string VaultName { get; set; }
        public DateTime VaultCreatedAt { get; set; }
        public VaultType VaultType { get; set; }
        public List<VaultKeyDto> Keys { get; set; }

    }
}
