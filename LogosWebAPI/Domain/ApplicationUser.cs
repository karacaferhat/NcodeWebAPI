using AspNetCore.Identity.MongoDbCore.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    public class ApplicationUser : MongoIdentityUser
    {

        public string City;
        public DateTime DateOfBirth;
        public string Instrument;
        public ApplicationUser()
        {
            
        }
    }
}
