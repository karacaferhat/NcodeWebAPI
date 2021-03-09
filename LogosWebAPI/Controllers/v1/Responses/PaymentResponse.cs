using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Requests
{
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }

        public string HtmlContent { get; set; }
    }
}
