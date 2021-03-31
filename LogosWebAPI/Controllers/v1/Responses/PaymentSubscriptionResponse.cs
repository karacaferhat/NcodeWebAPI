using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Requests
{
    public class PaymentSubscriptionResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string HtmlContent { get; set; }
        public string PaymentFormToken { get; set; }
        public string ConversationId { get; set; }
    }
}