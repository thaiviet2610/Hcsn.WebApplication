using Hcsn.WebApplication.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
    public class ValidateResult
    {
        public bool IsSuccess { get; set; }

        public ValidateCode? ValidateCode { get; set; }

        public string Message { get; set; }
    }
}
