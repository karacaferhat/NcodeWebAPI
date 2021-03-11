using System;
using System.Collections.Generic;
using NCodeWebAPI.Controllers.v1.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NCodeWebAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NCodeWebAPI.Contracts.v1;
using NCodeWebAPI.Controllers.v1.Requests;
using NCodeWebAPI.Domain;
using NCodeWebAPI.Services;



namespace NCodeWebAPI.Controllers.v1
{
   
    public class PaymentController : Controller
    {
        private IPaymentService _paymentService;
      
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
          
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(ApiRoutes.Payment.Post)]
        public async Task<IActionResult> PostPayment([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                PaymentPostRequest req = new PaymentPostRequest()
                {

                    UserId = HttpContext.GetUserId(),
                    Email = paymentRequest.Email,
                    Ip = paymentRequest.Ip
                };
                List<PaymentPostRequestItem> items = new List<PaymentPostRequestItem>() { };

                foreach (var item in paymentRequest.Items)
                {
                    PaymentPostRequestItem pi = new PaymentPostRequestItem()
                    {
                        Price = item.Price,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity

                    };
                    items.Add((pi));
                }

                req.Items = items;

                PaymentPostResponse serviceResponse = await _paymentService.PostPaymentAsync(req);

                if (serviceResponse.Success)
                {
                    return Ok(new PaymentResponse()
                    {
                        Success = true,
                        HtmlContent = serviceResponse.HtmlContent,
                        PaymentPageUrl = serviceResponse.PaymentPageUrl,
                        ConversationId = serviceResponse.ConversationId,
                        PaymentFormToken = serviceResponse.PaymentFormToken,
                        Error = serviceResponse.Error
                    });
                }

                return BadRequest(new PaymentResponse()
                {
                    Success = false,
                    HtmlContent = serviceResponse.HtmlContent,
                    PaymentPageUrl = serviceResponse.PaymentPageUrl,
                    ConversationId = serviceResponse.ConversationId,
                    PaymentFormToken = serviceResponse.PaymentFormToken,
                    Error = serviceResponse.Error
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new PaymentResponse()
                {
                    Success = false,
                    Error = ex.Message
                });
            }
          

        }

        

        [HttpGet(ApiRoutes.Payment.FetchPayment)]
        public async Task<IActionResult> FetchPayment([FromRoute] string payToken)
        {
            
            var paymentCheckResultReq = new PaymentCheckResultRequest()
            {
                payToken = payToken

            };
            PaymentCheckResultResponse serviceResponse = await _paymentService.PaymentCheckResultAsync(paymentCheckResultReq);
            if (serviceResponse.success)
            {
                return Ok(new PaymentCheckResponse()
                {
                    errorMessage = serviceResponse.errorMessage,
                    paymentId = serviceResponse.paymentId,
                    paymentStatus = serviceResponse.paymentStatus,
                    lastFourDigits = serviceResponse.lastFourDigits,
                    error = serviceResponse.error,
                    success = serviceResponse.success,
                });
            }

            return BadRequest(new PaymentCheckResponse()
            {
                errorMessage = serviceResponse.errorMessage,
                paymentId = serviceResponse.paymentId,
                paymentStatus = serviceResponse.paymentStatus,
                lastFourDigits = serviceResponse.lastFourDigits,
                error = serviceResponse.error,
                success = serviceResponse.success,
            });
            
        }

    }
   

}