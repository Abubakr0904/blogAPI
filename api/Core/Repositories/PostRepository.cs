using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Core.IRepositories;
using api.Data;
using api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace api.Core.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(BlogDbContext context, ILogger logger)
            :base(context, logger)
        {
            
        }
        public override async Task<IEnumerable<Post>> GetAll()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method error", typeof(PostRepository));
                return new List<Post>();
            }
        }

        public override async Task<Post> GetById(Guid id)
        {
            try
            {
                return await _dbSet.Include(p => p.Medias)
                        .Include(p => p.Comments)
                        .Where(p => p.Id == id)
                        .FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetById method error", typeof(PostRepository));
                return default(Post);
            }
        }

        public override async Task<bool> Update(Post entity)
        {
            try
            {
                var existingPost = await _dbSet.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();

                if (existingPost == default)
                    return false;
                
                existingPost.HeaderImageId = entity.HeaderImageId;
                existingPost.Title = entity.Title;
                existingPost.Description = entity.Description;
                existingPost.Content = entity.Content;
                existingPost.ModifiedAt = DateTimeOffset.UtcNow;
                existingPost.Comments = entity.Comments;
                existingPost.Medias = entity.Medias;

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} Update method error", typeof(PostRepository));
                return false;
            }
        }

        public override async Task<bool> Add(Post entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Add method error", typeof(PostRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var exist = await _dbSet.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (exist != default)
                {
                    _dbSet.Remove(exist);
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete method error", typeof(PostRepository));
                return false;
            }
        }


    }
}