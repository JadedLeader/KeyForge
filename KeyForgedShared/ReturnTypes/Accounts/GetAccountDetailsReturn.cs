using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Accounts
{
    public class GetAccountDetailsReturn
    {

        public string Username { get; set; }

        public string Email { get; set; }

        public string AccountCreated { get; set; }  

        public bool Success { get; set; }
    }
}
