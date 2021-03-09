using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCodeWebAPI.Controllers.v1.Responses;

namespace NCodeWebAPI.Domain
{
    public class AuthanticationResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserInfo userInfo { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }

      
    }
}
