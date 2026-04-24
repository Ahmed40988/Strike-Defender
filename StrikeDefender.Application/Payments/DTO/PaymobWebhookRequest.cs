using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Payments.DTO
{
    public class PaymobWebhookRequest
    {
        public PaymobWebhookObj Obj { get; set; } = default!;
    }

    public class PaymobWebhookObj
    {
        public int Id { get; set; } // transaction id

        public bool Success { get; set; }

        public int Amount_Cents { get; set; }

        public PaymobOrder Order { get; set; } = default!;
    }

    public class PaymobOrder
    {
        public int Id { get; set; } // ده المهم 👈 orderId
    }
}