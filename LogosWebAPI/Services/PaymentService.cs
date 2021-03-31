using NCodeWebAPI.Domain;
using System;
using System.Net;
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
using Iyzipay.Model.V2.Subscription;
using Iyzipay.Request.V2.Subscription;
using Iyzipay.Model.V2;
using System.Net.Http;
using System.Text;

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
                CallbackUrl = _settings.CallbackUrl + "?conversationId=" + ConversationId
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

                BasketItem firstBasketItem = new BasketItem
                {
                    Id = item.ProductId,
                    Name = item.ProductName,
                    Category2 = item.Academy,
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


            try
            {
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

        public async Task<PaymentCheckResultResponse> PaymentCheckResultAsync(PaymentCheckResultRequest request)
        {
            Iyzipay.Request.RetrieveCheckoutFormRequest r = new Iyzipay.Request.RetrieveCheckoutFormRequest()
            {
                ConversationId = request.conversationId,
                Token = request.payToken
            };
         
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
                  Email = request.email,
                  ErrorCode = checkoutForm.ErrorCode,
                  ErrorMessage = checkoutForm.ErrorMessage,
                  PaymentId = checkoutForm.PaymentId,
                  PaymentStatus = checkoutForm.PaymentStatus,
                  Status = checkoutForm.Status,
                  PaidPrice = checkoutForm.PaidPrice,
                  SysDate = DateTime.Now,
                  UserId = request.email,
                };



                var user = await _userManager.FindByEmailAsync(request.email);
                if (user == null)
                {
                    return new PaymentCheckResultResponse()
                    {
                        error = "",
                        errorMessage = "Kullanıcı Bulunamadı",
                        success = false,
                        lastFourDigits = checkoutForm.LastFourDigits,
                        paymentId = checkoutForm.PaymentId,
                        paymentStatus = checkoutForm.PaymentStatus
                    };

                }

                await _mongoService.CreatePaymentDocumentAsync(post);

                string profil = request.productName.Substring(0,1).ToUpper();
                user.Instrument = request.academy;
                user.Profile = profil;
                await _userManager.UpdateAsync(user);



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
               
        public async Task<PaymentSubscriptionServiceResponse> PostSubscriptionPaymentAsync(PaymentSubscriptionServiceRequest paymentRequest)
        {


            var user = await _userManager.FindByEmailAsync(paymentRequest.Email);

            string ConversationId = Guid.NewGuid().ToString();
           
            

            InitializeCheckoutFormRequest request = new InitializeCheckoutFormRequest
            {
                Locale = Locale.TR.ToString(),
                Customer = new CheckoutFormCustomer
                {
                    Email = paymentRequest.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    BillingAddress = new Address
                    {
                        City = user.City,
                        Country = "Türkiye",
                        Description = "Ödeme Adres",
                        ContactName = user.Email,
                        ZipCode = "35000"
                    },
                    ShippingAddress = new Address
                    {
                        City = user.City,
                        Country = "Türkiye",
                        Description = "Teslimat Adres",
                        ContactName = user.Email,
                        ZipCode = "35000"
                    },
                    GsmNumber = "+905350000000",
                    IdentityNumber = "55555555555",
                },
                CallbackUrl = _settings.CallbackUrl + "?conversationId=" + ConversationId+"&email="+user.Email+ "&productName="+ paymentRequest.ProductName+"&academy="+ paymentRequest.Academy,
                ConversationId = ConversationId+user.Email,
                PricingPlanReferenceCode = paymentRequest.ProductId,
                SubscriptionInitialStatus = SubscriptionStatus.ACTIVE.ToString()
            };

            Iyzipay.Options options = new Iyzipay.Options()
            {
                ApiKey = _settings.ApiKey,
                BaseUrl = _settings.Url,
                SecretKey = _settings.Secret
            };


            try
            {
                CheckoutFormResource response = Subscription.InitializeCheckoutForm(request, options);

                if (response.Status == Status.SUCCESS.ToString())
                {
                    return new PaymentSubscriptionServiceResponse()
                    {
                        Error = null,
                        Success = true,
                        HtmlContent = response.CheckoutFormContent,                        
                        ConversationId = response.ConversationId,
                        PaymentFormToken = response.Token
                       
                    };



                }


                return new PaymentSubscriptionServiceResponse()
                {
                    Error = response.ErrorMessage,
                    Success = false,
                    HtmlContent = response.CheckoutFormContent,
                    ConversationId = response.ConversationId,
                    PaymentFormToken = response.Token
     
                };
            }
            catch (Exception ex)
            {
                return new PaymentSubscriptionServiceResponse()
                {
                    Error = ex.Message,
                    Success = false

                };
            }


        }

        public async Task<PaymentCheckSubscriptionResultResponse> SubscriptionPaymentCheckResultAsync(PaymentCheckSubscriptionResultRequest request)
        {
            PaymentCheckResultRequest req = new PaymentCheckResultRequest
            {
                conversationId = request.conversationId,
                payToken = request.payToken,
                email=request.email,
                academy=request.academy,
                productName=request.productName
               
            };
            PaymentCheckResultResponse resp= await PaymentCheckResultAsync(req);

            PaymentCheckSubscriptionResultResponse response = new PaymentCheckSubscriptionResultResponse
            {
                error = resp.error,
                errorMessage = resp.errorMessage,
                subscriptionStatus = resp.paymentStatus,
                referenceCode = resp.paymentId,
                success = resp.success

            };
            return response;
        }

        //public async Task<PaymentCheckSubscriptionResultResponse> SubscriptionPaymentCheckResultAsync(PaymentCheckSubscriptionResultRequest request)
        //{
        //    Iyzipay.Options options = new Iyzipay.Options()
        //    {
        //        ApiKey = _settings.ApiKey,
        //        BaseUrl = _settings.Url,
        //        SecretKey = _settings.Secret
        //    };

        //    Iyzipay.Request.V2.Subscription.CheckoutFormResultRequest req = new Iyzipay.Request.V2.Subscription.CheckoutFormResultRequest
        //    {
        //        Token = request.payToken,
        //        Locale=Locale.TR.ToString()
        //    };


        //   ResponseData<SubscriptionCreatedResource> response = Subscription.InitializeCheckoutFormResponse(req, options);




        //    if (response.Status == Status.SUCCESS.ToString())
        //    {
        //        RetrieveCustomerRequest retrieveCustomerRequest = new RetrieveCustomerRequest
        //        {
        //            Locale = Locale.TR.ToString(),
        //            ConversationId = request.conversationId,
        //            CustomerReferenceCode = response.Data.CustomerReferenceCode
        //        };

        //        ResponseData<CustomerResource> customerResponse = Customer.Retrieve(retrieveCustomerRequest, options);

        //        PaymentDocument post = new PaymentDocument
        //        {
        //            Email = customerResponse.Data.Email,
        //            ErrorCode = "",
        //            ErrorMessage = "",
        //            PaymentId = response.Data.PricingPlanReferenceCode,
        //            PaymentStatus = response.Data.SubscriptionStatus,
        //            Status = response.Status,
        //            PaidPrice = "0",
        //            SysDate = DateTime.Now,
        //            UserId = customerResponse.Data.Email,
        //        };



        //        var user = await _userManager.FindByEmailAsync(customerResponse.Data.Email);
        //        if (user == null)
        //        {
        //            return new PaymentCheckSubscriptionResultResponse()
        //            {
        //                error = "",
        //                errorMessage = "Kullanıcı Bulunamadı",
        //                success = false
        //            };

        //        }

        //        await _mongoService.CreatePaymentDocumentAsync(post);

        //        string profil = "";
        //        string academy = "";

        //        user.Instrument = academy;
        //        user.Profile = profil.ToUpper();
        //        await _userManager.UpdateAsync(user);





        //        return new PaymentCheckSubscriptionResultResponse()
        //        {
        //            error = "",
        //            errorMessage = "",
        //            success = true,
        //            referenceCode = response.Data.ReferenceCode,
        //            subscriptionStatus = response.Data.SubscriptionStatus
        //        };
        //    }
        //    return new PaymentCheckSubscriptionResultResponse()
        //    {
        //        error = response.ErrorMessage ,
        //        errorMessage = response.Status,
        //        success = false,
        //        referenceCode = "",
        //        subscriptionStatus = response.Data.SubscriptionStatus
        //    };
        //}
    }
}
