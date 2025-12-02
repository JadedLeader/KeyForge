using KeyForgedShared.SharedDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ValidationType
{
    public class UpdateTeamInviteValidationResult
    {

        public TeamInviteDataModel TeamInvite { get; set; }

        public bool IsValidated { get; set; }

    }
}
