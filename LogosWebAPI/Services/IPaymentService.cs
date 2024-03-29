﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCodeWebAPI.Domain;

namespace NCodeWebAPI.Services
{
    public interface IPaymentService
    {
        Task<PaymentPostResponse> PostPaymentAsync(PaymentPostRequest request);
        Task<PaymentCheckResultResponse> PaymentCheckResultAsync(PaymentCheckResultRequest request);

        Task<PaymentSubscriptionServiceResponse> PostSubscriptionPaymentAsync(PaymentSubscriptionServiceRequest request);

        Task<PaymentCheckSubscriptionResultResponse> SubscriptionPaymentCheckResultAsync(PaymentCheckSubscriptionResultRequest request);
    }
}
