using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Controllers.v1.Responses
{
    public class ChangePasResponse
    {
        public Boolean IsSuccesfull { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
