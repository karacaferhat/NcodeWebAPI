using NCodeWebAPI.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NCodeWebAPI.Options;
using Iyzipay;
using Iyzipay.Request;
using Iyzipay.Model;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace NCodeWebAPI.Services
{
    public class PaymentService : IPaymentService

    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IyzicoPaymentSettings _settings;
        private IMongoPostService _mongoService;
        public PaymentService(UserManager<ApplicationUser> userManager,IyzicoPaymentSettings settings, IMongoPostService mongoService)
        {
            _settings = settings;
            _userManager = userManager;
            _mongoService = mongoService;
        }

        public async Task<PaymentCheckResultResponse> PaymentCheckResultAsync(PaymentCheckResultRequest request)
        {
            RetrieveCheckoutFormRequest r = new RetrieveCheckoutFormRequest();
            r.ConversationId = r.ConversationId;
            r.Token = request.payToken;

            Iyzipay.Options options = new Iyzipay.Options()
            {
                ApiKey = _settings.ApiKey,
                BaseUrl = _settings.Url,
                SecretKey = _settings.Secret
            };
            
            CheckoutForm checkoutForm =  CheckoutForm.Retrieve(r, options);
            if (checkoutForm.ErrorCode == null)
            {
                PaymentDocument post = new PaymentDocument
                {
                  Email = checkoutForm.BasketId,
                  ErrorCode = checkoutForm.ErrorCode,
                  ErrorMessage = checkoutForm.ErrorMessage,
                  PaymentId = checkoutForm.PaymentId,
                  PaymentStatus = checkoutForm.PaymentStatus,
                  Status = checkoutForm.Status,
                  PaidPrice = checkoutForm.PaidPrice,
                  SysDate = DateTime.Now,
                  UserId = checkoutForm.BasketId,
                };
                await _mongoService.CreatePaymentDocumentAsync(post);

                return new PaymentCheckResultResponse()
                {
                    error = "",
                    success = true,
                    lastFourDigits = checkoutForm.LastFourDigits,
                    paymentId = checkoutForm.PaymentId,
                    paymentStatus= checkoutForm.PaymentStatus
                };
            }

            return new PaymentCheckResultResponse()
            {
                error = checkoutForm.ErrorCode,
                errorMessage =  checkoutForm.ErrorMessage,
                success = false,
                lastFourDigits = checkoutForm.LastFourDigits,
                paymentId = checkoutForm.PaymentId,
                paymentStatus = checkoutForm.PaymentStatus
            };



        }

        public async Task<PaymentPostResponse> PostPaymentAsync(PaymentPostRequest paymentRequest)
        {
            var user = await _userManager.FindByEmailAsync(paymentRequest.Email);

            string ConversationId = Guid.NewGuid().ToString();
            decimal totalPrice = 0;
            foreach (var item in paymentRequest.Items)
            {
                 totalPrice = totalPrice + item.Price;
            }

           

           

            CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = ConversationId,
                Price = totalPrice.ToString(),
                PaidPrice = totalPrice.ToString(),
                Currency = Currency.TRY.ToString(),
                BasketId = user.Email,
                PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                CallbackUrl = _settings.CallbackUrl+"?conversationId="+ ConversationId
            };

            List<int> enabledInstallments = new List<int>();
            enabledInstallments.Add(2);
            enabledInstallments.Add(3);
            enabledInstallments.Add(6);
            enabledInstallments.Add(9);
            request.EnabledInstallments = enabledInstallments;

            Buyer buyer = new Buyer
            {
                Id = paymentRequest.UserId,
                Name = user.Name,
                Surname = user.Surname,
                Email = paymentRequest.Email,
                IdentityNumber = _settings.DefaultBuyerId,
                RegistrationAddress = _settings.DefaultBuyerAddress,
                Ip = paymentRequest.Ip,
                City = user.City,
                Country = "Turkey",
               
            };
            request.Buyer = buyer;

            Address shippingAddress = new Address
            {
                ContactName = buyer.Name,
                City = buyer.City,
                Country = buyer.Country,
                Description = buyer.RegistrationAddress,
                ZipCode = buyer.ZipCode
            };
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address
            {
                ContactName = shippingAddress.ContactName,
                City = shippingAddress.City,
                Country = shippingAddress.Country,
                Description = shippingAddress.Description,
                ZipCode = shippingAddress.ZipCode
            };
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            foreach (var item in paymentRequest.Items)
            {
                 BasketItem firstBasketItem = new BasketItem {
                        Id = item.ProductId,
                        Name = item.ProductName,
                        Category1 = item.ProductName,
                        ItemType = BasketItemType.PHYSICAL.ToString(),
                        Price = item.Price.ToString()
                 };

                 basketItems.Add(firstBasketItem);
            }
           



            request.BasketItems = basketItems;


            Iyzipay.Options options = new Iyzipay.Options()
            {
                ApiKey = _settings.ApiKey,
                BaseUrl = _settings.Url,
                SecretKey = _settings.Secret
            };


            try {
                CheckoutFormInitialize checkoutFormInitialize = CheckoutFormInitialize.Create(request, options);


                if (checkoutFormInitialize.ErrorCode == null)
                {
                    return new PaymentPostResponse()
                    {
                        Error = null,
                        Success = true,
                        HtmlContent = checkoutFormInitialize.CheckoutFormContent,
                        PaymentPageUrl = checkoutFormInitialize.PaymentPageUrl,
                        ConversationId = checkoutFormInitialize.ConversationId,
                        PaymentFormToken = checkoutFormInitialize.Token,
                        details = checkoutFormInitialize
                    };



                }


                return new PaymentPostResponse()
                {
                    Error = checkoutFormInitialize.ErrorMessage,
                    Success = false,
                    HtmlContent = checkoutFormInitialize.CheckoutFormContent,
                    PaymentPageUrl = checkoutFormInitialize.PaymentPageUrl,
                    ConversationId = checkoutFormInitialize.ConversationId,
                    PaymentFormToken = checkoutFormInitialize.Token,
                    details = checkoutFormInitialize
                };
            }
            catch (Exception ex)
            {
                return new PaymentPostResponse()
                {
                    Error = ex.Message,
                    Success = false

                };
            }

          


        }

    }
}
