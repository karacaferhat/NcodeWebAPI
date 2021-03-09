using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Requests
{
    public class UserInfoUpdateRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Profile { get; set; }

        public string City { get; set; }

        public string Instrument { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
