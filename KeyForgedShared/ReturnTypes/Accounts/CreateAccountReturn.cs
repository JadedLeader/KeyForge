using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Accounts
{
    public class CreateAccountReturn
    {

        public string Username { get; set; }

        public string Password { get; set; }

        public bool Success { get; set; }

    }
}
