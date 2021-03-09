using Iyzipay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    public class PaymentPostResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }

        public string HtmlContent { get; set; }

        public CheckoutFormInitialize details { get; set; }
    }
}
