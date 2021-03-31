using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Responses
{
    public class PaymentCheckSubscriptionResponse
    {
        public bool success { get; set; }
        public string error { get; set; }
        public string errorMessage { get; set; }
        public string referenceCode { get; set; }
        public string subscriptionStatus { get; set; }        
    }
}
