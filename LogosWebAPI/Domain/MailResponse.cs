using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    public class MailResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
