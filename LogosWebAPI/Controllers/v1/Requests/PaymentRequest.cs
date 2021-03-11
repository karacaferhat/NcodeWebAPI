using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Requests
{
    public class PaymentRequest
    {
        public string Email { get; set; }
        public string Ip { get; set; }
        public List<PaymentRequestItem> Items { get; set; }

}

    public class PaymentRequestItem
    {
        public string ProductName { get; set; }
        public string ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }
    }
   
}
