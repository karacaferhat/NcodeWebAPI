using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NCodeWebAPI.Domain;

namespace NCodeWebAPI.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync();

        Task<Post> GetPostByIdAsync(Guid postId);

        Task<bool> UpdatePostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Guid postId);

        Task<bool> CreatePostAsync(Post post);

        Task<bool> UserOwnPostAsync(Guid postId, string getUserId);
    }
}
