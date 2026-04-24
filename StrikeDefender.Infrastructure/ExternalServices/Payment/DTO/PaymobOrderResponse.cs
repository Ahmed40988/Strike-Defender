using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.Payment.DTO
{
    // response from paymob api contain order id only to be used in payment key request to get the token for payment process
    public record PaymobOrderResponse (int Id );
 
}
