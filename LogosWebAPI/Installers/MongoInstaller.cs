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
using AspNetCore.Identity.Mongo;

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
            var database = client.GetDatabase(mongoDbSettings.databaseName);
            var userCollection = database.GetCollection<ApplicationUser>(mongoDbSettings.userCollectionName);
            services.AddSingleton(userCollection);
            var tokenCollection = database.GetCollection<RefreshToken>(mongoDbSettings.tokenCollectionName);
            services.AddSingleton(tokenCollection);





            _ = services.AddIdentity<ApplicationUser, MongoIdentityRole>(
                    i =>
                    {
                        i.Password.RequiredLength = 6;
                        i.Password.RequireLowercase = false;
                        i.Password.RequireNonAlphanumeric = false;
                        i.Password.RequireUppercase = false;
                        i.Password.RequiredUniqueChars = 1;
                    })
                    .AddMongoDbStores<ApplicationUser, MongoIdentityRole, Guid>(mongoDbSettings.connectionUrl, mongoDbSettings.databaseName)
                    .AddSignInManager()
                    .AddDefaultTokenProviders();

            services.AddScoped<IMongoPostService, MongoPostService>();
           
        }
    }
}
