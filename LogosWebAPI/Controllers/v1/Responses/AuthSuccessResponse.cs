using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Responses
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }

        public string RefreshToken{ get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Profile { get; set; }
    }
}
