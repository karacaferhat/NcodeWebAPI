using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    public class PaymentCheckSubscriptionResultResponse
    {
        public bool success { get; set; }
        public string error { get; set; }
        public string errorMessage { get; set; }
        public string referenceCode { get; set; }
        public string subscriptionStatus { get; set; }

        
    }
}
