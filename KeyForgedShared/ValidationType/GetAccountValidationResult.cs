using KeyForgedShared.SharedDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyForgedShared.ValidationType
{
    public class GetAccountValidationResult
    {

        public AccountDataModel AccountModel { get; set; }

        public bool IsValidated { get; set; }

    }
}
