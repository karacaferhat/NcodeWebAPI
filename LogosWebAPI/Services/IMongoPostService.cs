using System.Collections.Generic;
using System.Threading.Tasks;
using NCodeWebAPI.Domain;


namespace NCodeWebAPI.Services
{
    public interface IMongoPostService
    {
        Task<List<MongoPost>> GetPostsAsync();


        Task<MongoPost> CreatePostAsync(MongoPost post);

        Task<PaymentDocument> CreatePaymentDocumentAsync(PaymentDocument post);


    }
}


