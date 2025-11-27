using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Auth
{
    public class SilentShortLivedTokenRefreshReturn
    {

        public string RefreshedShortLivedToken { get ; set; }

        public bool Success { get; set; }

    }
}
