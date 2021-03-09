using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using NCodeWebAPI.Domain;
using NCodeWebAPI.Options;

namespace NCodeWebAPI.Services
{
    

    public class MongoPostService : IMongoPostService

    {
        private readonly IMongoCollection<MongoPost> _mongoPosts;

        private MongoDbSettings _mongoDbSettings;
        public MongoPostService(MongoDbSettings  mongoDbSettings )
        {
            _mongoDbSettings = mongoDbSettings;
            var client = new MongoClient(mongoDbSettings.connectionUrl);
            var database = client.GetDatabase(mongoDbSettings.databaseName);
             _mongoPosts = database.GetCollection<MongoPost>("Posts");
          
        }


        public async Task<List<MongoPost>> GetPostsAsync()
        {
          return  await _mongoPosts.Find(mp => true).ToListAsync();
        }

        public async Task<MongoPost> CreatePostAsync(MongoPost post)
        {
            await  _mongoPosts.InsertOneAsync(post);
            return post;
        }

      



    }
}
