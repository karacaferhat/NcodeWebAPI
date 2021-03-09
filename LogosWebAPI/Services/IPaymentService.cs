using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCodeWebAPI.Domain;

namespace NCodeWebAPI.Services
{
    public interface IPaymentService
    {
        Task<PaymentPostResponse> PostPaymentAsync(PaymentPostRequest request);
    }
}
