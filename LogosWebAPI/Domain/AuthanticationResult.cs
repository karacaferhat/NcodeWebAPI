using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    public class AuthanticationResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Profile { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }


    }
}
