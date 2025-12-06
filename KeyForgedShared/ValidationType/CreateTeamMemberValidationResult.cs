using KeyForgedShared.SharedDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ValidationType
{
    public class CreateTeamMemberValidationResult
    {

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsValidated { get; set; }

    }
}
