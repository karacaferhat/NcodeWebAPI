using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Requests
{
    public class PaymentSubscriptionRequest
    {
        public string Email { get; set; }
        public string Ip { get; set; }
        public string ProductName { get; set; }
        public string Academy { get; set; }
        public string ProductId { get; set; }
    }
}




