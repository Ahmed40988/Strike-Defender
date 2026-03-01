using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Domain.Common.Enums
{
    public enum AttackCategory
    {
        Injection = 1,
        Authentication = 2,
        Authorization = 3,
        FileHandling = 4,
        Misconfiguration = 5
    }

}
