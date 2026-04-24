using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IPaymentService
    {
            Task<ErrorOr<string>> ProcessPaymentAsync(AppUser user, Plan plan);

    }
}
