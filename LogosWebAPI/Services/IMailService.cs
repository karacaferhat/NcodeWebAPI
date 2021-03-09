using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCodeWebAPI.Domain;

namespace NCodeWebAPI.Services
{
    public interface IMailService
    {
        Task<MailResponse> SendMailAsync(string to,string subject,string body);
    }
}
