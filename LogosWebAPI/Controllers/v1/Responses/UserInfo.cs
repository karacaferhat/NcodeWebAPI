using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Responses
{
    public class UserInfo
    {
        public string EMail { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Profile { get; set; }

        public string City { get; set; }

        public string Instrument { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
