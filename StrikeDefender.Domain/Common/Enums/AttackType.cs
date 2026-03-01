using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Domain.Common.Enums
{
    public enum AttackType
    {
        XSS = 1,
        SQLInjection = 2,
        CommandInjection = 3,
        FileUpload = 4,
        AuthenticationBypass = 5,
        Unknown = 99
    }

}
