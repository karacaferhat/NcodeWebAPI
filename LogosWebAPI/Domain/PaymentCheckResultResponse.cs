using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    public class PaymentCheckResultResponse
    {
        public bool success { get; set; }
        public string error { get; set; }
        public string errorMessage { get; set; }
        public string paymentId { get; set; }
        public string paymentStatus { get; set; }

        public string lastFourDigits { get; set; }
    }
}
