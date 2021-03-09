using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    public class PaymentPostRequest
    {

        public string UserId { get; set; }
        public string Email { get; set; }

        public string Ip { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }

        public Decimal Quantity { get; set; }

        public Decimal Price { get; set; }


    }

    
}
