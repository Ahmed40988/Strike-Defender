using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.Payment.DTO
{
    // token for Authentication backend  on Paymob to accsss the api and make order and payment key request to get the token for payment process
    public record PaymobAuthResponse (string Token );

}
