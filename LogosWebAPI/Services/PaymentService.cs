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

        public PaymentService(UserManager<ApplicationUser> userManager,IyzicoPaymentSettings settings)
        {
            _settings = settings;
            _userManager = userManager;
        }

        public async Task<PaymentPostResponse> PostPaymentAsync(PaymentPostRequest paymentRequest)
        {
            var user = await _userManager.FindByEmailAsync(paymentRequest.Email);

            string basketId = Guid.NewGuid().ToString();

            decimal totalPrice= paymentRequest.Price;

           

            CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = basketId,
                Price = totalPrice.ToString(),
                PaidPrice = totalPrice.ToString(),
                Currency = Currency.TRY.ToString(),
                BasketId = basketId,
                PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                CallbackUrl = _settings.CallbackUrl
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

            BasketItem firstBasketItem = new BasketItem
            {
                Id = paymentRequest.ProductId,
                Name = paymentRequest.ProductName,
                Category1 = paymentRequest.ProductName,
                ItemType = BasketItemType.PHYSICAL.ToString(),
                Price = paymentRequest.Price.ToString()
            };
            basketItems.Add(firstBasketItem);



            request.BasketItems = basketItems;


            Iyzipay.Options options = new Iyzipay.Options()
            {
                ApiKey = _settings.ApiKey,
                BaseUrl = _settings.Url,
                SecretKey = _settings.Secret
            };



            CheckoutFormInitialize checkoutFormInitialize = CheckoutFormInitialize.Create(request, options);


            if (checkoutFormInitialize.ErrorCode == null)
            {
                return new PaymentPostResponse()
                {
                    Error = null,
                    Success = true,
                    HtmlContent = checkoutFormInitialize.CheckoutFormContent,
                    details = checkoutFormInitialize
                };



            }


            return new PaymentPostResponse()
            {
                Error = checkoutFormInitialize.ErrorMessage,
                Success = false,
                HtmlContent = checkoutFormInitialize.CheckoutFormContent,
                details = checkoutFormInitialize
            };


        }
    }
}
