using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.Payment.DTO
{
    // contain all information about order and token response from paymob api
    public record PaymobPaymentKeyResponse ( string Token);
}
