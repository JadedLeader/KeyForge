using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Auth
{
    public class RefreshLongLivedTokenReturn
    {

        public string AccountId { get; set; }

        public string RefreshedToken { get; set; }

        public bool Success { get; set; }

    }
}
