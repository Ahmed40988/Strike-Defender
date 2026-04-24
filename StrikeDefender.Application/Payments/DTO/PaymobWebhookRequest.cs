using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Payments.DTO
{
    public record PaymobWebhookRequest(
       string Type,
       int OrderId,
       bool Success,
       decimal AmountCents,
       string Hmac
   );

    public class PaymobWebhookObj
        {
            public int OrderId { get; set; }          
            public bool Success { get; set; }        
            public decimal AmountCents { get; set; }  
            public string Hmac { get; set; } = string.Empty;
        }
    }
