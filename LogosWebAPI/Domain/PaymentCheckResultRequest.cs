﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    public class PaymentCheckResultRequest
    {
        public string conversationId { get; set; }
        public string payToken { get; set; }

        public string email { get; set; }
        public string productName { get; set; }

        public string academy { get; set; }

    }
}
