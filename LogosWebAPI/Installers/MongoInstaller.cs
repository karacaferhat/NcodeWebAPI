using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using NCodeWebAPI.Domain;
using NCodeWebAPI.Options;
using NCodeWebAPI.Services;

namespace NCodeWebAPI.Installers
{
    public class MongoInstaller : IInstaller

    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings = new MongoDbSettings();
            configuration.Bind(nameof(mongoDbSettings), mongoDbSettings);
            services.AddSingleton(mongoDbSettings);
            var client = new MongoClient(mongoDbSettings.connectionUrl);
            var database = client.GetDatabase("MongoIdentityDb");
            var userCollection = database.GetCollection<ApplicationUser>("applicationUser");
            services.AddSingleton(userCollection);
            var tokenCollection = database.GetCollection<RefreshToken>("applicationTokens");
            services.AddSingleton(tokenCollection);



            services.AddIdentity<ApplicationUser, MongoIdentityRole>()
                .AddMongoDbStores<ApplicationUser, MongoIdentityRole, Guid>(mongoDbSettings.connectionUrl, "MongoIdentityDb")
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddScoped<IMongoPostService, MongoPostService>();
        }
    }
}
