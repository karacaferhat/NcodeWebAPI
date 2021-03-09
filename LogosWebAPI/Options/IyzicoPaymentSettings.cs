using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Options
{
    public class IyzicoPaymentSettings
    {
        public string ApiKey { get; set; }
        public string Secret { get; set; }
        public string Url { get; set; }
        public string DefaultBuyerAddress { get; set; }
        public string DefaultBuyerId { get; set; }

    }
}
