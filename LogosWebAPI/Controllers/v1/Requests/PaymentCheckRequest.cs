using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Responses
{
    public class PaymentCheckRequest
    {
        public string conversationId { get; set; }
        public string payToken { get; set; }
    }
}
