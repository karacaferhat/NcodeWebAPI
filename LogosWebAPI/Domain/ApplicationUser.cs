using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Domain
{
    [CollectionName("UsersTable")]
    public class ApplicationUser : MongoIdentityUser
    {

        public string City;
        public DateTime DateOfBirth;
        public string Instrument;
        public string Name;
        public string Surname;
        public string Profile;
        public ApplicationUser()
        {
            
        }
    }
}
