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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentController : Controller
    {
        private IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        [HttpPost(ApiRoutes.Payment.Post)]
        public async Task<IActionResult> PostPayment([FromBody] PaymentRequest paymentRequest)
        {
            PaymentPostRequest req = new PaymentPostRequest()
            {

                UserId = HttpContext.GetUserId(),
                Email = paymentRequest.Email,
                Ip = paymentRequest.Ip,
                Price = paymentRequest.Price,
                ProductId = paymentRequest.ProductId,
                ProductName = paymentRequest.ProductName,
                Quantity = paymentRequest.Quantity,
            };

           PaymentPostResponse serviceResponse =  await _paymentService.PostPaymentAsync(req);

           if (serviceResponse.Success)
          {
              return Ok( new PaymentResponse()
              {
                  Success = true,
                  HtmlContent = serviceResponse.HtmlContent,
                  Error = serviceResponse.Error
              });
            }

           return BadRequest(new PaymentResponse()
          {
              Success = false,
              HtmlContent = serviceResponse.HtmlContent,
              Error = serviceResponse.Error
          });

        }

    }
   

}