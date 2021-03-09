using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using NCodeWebAPI.Options;
using NCodeWebAPI.Services;
using Microsoft.AspNetCore.Identity;
using NCodeWebAPI.Domain;

namespace NCodeWebAPI.Installers
{
    public class PaymentServiceInstaller:IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var IyzicoPaymentSettings = new IyzicoPaymentSettings();
            configuration.Bind(nameof(IyzicoPaymentSettings), IyzicoPaymentSettings);
            services.AddSingleton(IyzicoPaymentSettings);

         
            services.AddScoped<IPaymentService, PaymentService>();
        }
    }
}
