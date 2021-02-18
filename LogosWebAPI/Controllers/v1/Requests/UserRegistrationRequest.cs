using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace NCodeWebAPI.Controllers.v1.Requests
{
    public class UserRegistrationRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        public string Instrument { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
