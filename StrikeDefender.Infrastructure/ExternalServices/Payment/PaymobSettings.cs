using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.Payment
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; }
        public int IntegrationId { get; set; }
        public int IframeId { get; set; }
        public string BaseUrl { get; set; }
        public string Publickey { get; set; }
        public string Secretkey { get; set; }
        public string HMAC { get; set; }
    }
}
