
using Microsoft.AspNetCore.Mvc;
using NCodeWebAPI.Domain;
using System.Collections.Generic;
using System;
using NCodeWebAPI.Contracts.v1;

namespace NCodeWebAPI.Controllers.V1
{
    public class UserController : Controller
    {
        [HttpGet(ApiRoutes.PathList.GetUserList)]
        public IActionResult GetUserList()
        {
            return Ok(new { name = "Ferhat" });
        }

      


    }
}
