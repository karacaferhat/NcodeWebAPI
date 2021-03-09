using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCodeWebAPI.Options;
using System.Net.Mail;
using System.Net;
using NCodeWebAPI.Services;

namespace NCodeWebAPI.Installers
{
    public class MailServiceInstaller : IInstaller

    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var smtpSettings = new SmtpSettings();
            configuration.Bind(nameof(smtpSettings), smtpSettings);
            services.AddSingleton(smtpSettings);

            var smtpClient = new SmtpClient(smtpSettings.host)
            {
                Port = smtpSettings.port,
                Credentials = new NetworkCredential(smtpSettings.username, smtpSettings.password),
                EnableSsl = true,
            };

            services.AddSingleton(smtpClient);
            services.AddSingleton<IMailService, MailService>();
        }
    }
}
