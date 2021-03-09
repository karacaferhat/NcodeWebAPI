using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Requests
{
    public class ResetPasRequest
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
