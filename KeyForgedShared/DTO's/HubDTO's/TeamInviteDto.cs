using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.DTO_s.HubDTO_s
{
    public class TeamInviteDto
    {

        public string TeamInviteId { get; set; }
        public string InviteSentBy { get; set; }

        public string InviteRecipient { get; set; }

        public string InviteStatus { get; set; }

        public string InviteCreatedAt {  get; set; }    

    }
}
