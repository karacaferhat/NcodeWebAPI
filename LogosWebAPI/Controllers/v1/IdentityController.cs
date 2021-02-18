using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCodeWebAPI.Contracts.v1;
using NCodeWebAPI.Controllers.v1.Requests;
using NCodeWebAPI.Controllers.v1.Responses;
using NCodeWebAPI.Services;

namespace NCodeWebAPI.Controllers.v1
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;

        }


        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult>Register([FromBody]UserRegistrationRequest request)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new AuthFailResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _identityService.RegisterAsync(request.Email, request.Password,request.City,request.DateOfBirth,request.Instrument,request.Name,request.Surname);
            if (!authResponse.Success)
            {
                return BadRequest( new AuthFailResponse
                {
                    Errors = authResponse.Errors
                });
            }
            return Ok(new AuthSuccessResponse {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken,
                Name = authResponse.Name,
                Surname = authResponse.Surname,
                Profile = authResponse.Profile
            } );
        }


        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = authResponse.Errors
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken=authResponse.RefreshToken,
                Name = authResponse.Name,
                Surname =authResponse.Surname,
                Profile= authResponse.Profile

            });
        }


        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = authResponse.Errors
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken,
                Name = authResponse.Name,
                Surname = authResponse.Surname,
                Profile = authResponse.Profile
            });
        }
    }
}
   
