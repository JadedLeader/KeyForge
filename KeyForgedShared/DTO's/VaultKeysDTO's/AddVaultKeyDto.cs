using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.DTO_s.VaultKeysDTO_s
{
    public class AddVaultKeyDto
    {

        public string KeyName { get; set; }   

        public string PasswordToEncrypt { get ; set; } 

        public string VaultId { get; set; }


    }
}
