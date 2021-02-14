using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NCodeWebAPI.Data;
using NCodeWebAPI.Domain;


namespace NCodeWebAPI.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _datacontext;

        public PostService(DataContext datacontext)
        {
            _datacontext = datacontext;
        }
        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return  await _datacontext.Posts.SingleOrDefaultAsync( x => x.Id== postId);

        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _datacontext.Posts.ToListAsync() ;
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
             _datacontext.Posts.Update(postToUpdate);
            var updated=await _datacontext.SaveChangesAsync();

            return updated > 0;

        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);
            if (post == null)
                return false;

            _datacontext.Posts.Remove(post);
            var deleted = await _datacontext.SaveChangesAsync();

            return deleted >0;


        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            await _datacontext.Posts.AddAsync(post);
            var created =await _datacontext.SaveChangesAsync();
            return created > 0;

        }

        public async Task<bool> UserOwnPostAsync(Guid postId, string userId)
        {
            var post = await _datacontext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);
            if (post==null)
            {
                return false;
            }
            if(post.UserId != userId)
            {
                return false;
            }
            return true;
        } 
    }
}
