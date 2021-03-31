using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    public class PaymentCheckSubscriptionResultRequest
    {
        public string conversationId { get; set; }
        public string payToken { get; set; }
        public string email { get; set; }
        public string academy { get; set; }
        public string productName { get; set; }
    }
}
