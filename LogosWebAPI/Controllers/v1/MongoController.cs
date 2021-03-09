using System;
using NCodeWebAPI.Controllers.v1.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NCodeWebAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NCodeWebAPI.Contracts.v1;
using NCodeWebAPI.Controllers.v1.Requests;
using NCodeWebAPI.Domain;
using NCodeWebAPI.Services;



namespace NCodeWebAPI.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MongoController : Controller
    {
        private IMongoPostService _mongoPostService;
        public MongoController(IMongoPostService mongoPostService)
        {
            _mongoPostService = mongoPostService;
        }


        [HttpGet(ApiRoutes.Mongo.Get)]
        public async Task<IActionResult> GetAll()
        {
         
           return Ok(await _mongoPostService.GetPostsAsync());
        }


        [HttpPost(ApiRoutes.Mongo.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMongoPostRequest postRequest)
        {

            MongoPost post = new MongoPost
            {
                Author=postRequest.Author,
                BookName = postRequest.BookName,
                Category = postRequest.Category
            };

            await _mongoPostService.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            
            return Ok();

        }

       

    }
}
