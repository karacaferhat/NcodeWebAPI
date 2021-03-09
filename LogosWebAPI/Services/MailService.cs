using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using NCodeWebAPI.Domain;
using NCodeWebAPI.Options;

namespace NCodeWebAPI.Services
{
    public class MailService : IMailService

    {
        private SmtpClient _smtpClient;
        private SmtpSettings _smtpSettings;

        public  MailService (SmtpClient smtpClient,SmtpSettings smtpSettings )
        {
            _smtpClient = smtpClient;
            _smtpSettings = smtpSettings;
        }

        public async Task<MailResponse> SendMailAsync(string to, string subject, string body)
        {
            
     
            try
            {

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(_smtpSettings.username);
                msg.To.Add(to);
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = true;


                await Task.Run(() => _smtpClient.SendAsync(msg, null));

                return new MailResponse
                {
                    Success = true,
                    Error = ""
                };
            }
            catch (Exception e)
            {
                return new MailResponse
                {
                    Success = false,
                    Error = e.Message
                };

            }
        }
    }
}
