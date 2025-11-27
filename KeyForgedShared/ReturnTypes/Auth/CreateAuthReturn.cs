using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ReturnTypes.Auth
{
    public class CreateAuthReturn
    {

        public string AccountId { get; set; }

        public string ShortLivedToken { get; set; }

        public string LongLivedToken { get; set; }

        public bool Success { get; set; }

    }
}
