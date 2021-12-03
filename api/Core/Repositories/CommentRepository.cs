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
    public class CommentRepository: GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(BlogDbContext context, ILogger logger)
            :base(context, logger)
        {
            
        }

        public override async Task<IEnumerable<Comment>> GetAllById(Guid id)
        {
            try
            {
                return await _dbSet
                        .Where(p => p.PostId == id)
                        .ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAllById method error", typeof(CommentRepository));
                return new List<Comment> ();
            }
        }

        public override async Task<bool> Add(Comment entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Add method error", typeof(CommentRepository));
                return false;
            }
        }

        public override async Task<bool> Update(Comment entity)
        {
            try
            {
                var existingComment = await _dbSet.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();

                if (existingComment == default)
                    return false;
                
                existingComment.Author = entity.Author;
                existingComment.Content  = entity.Content;
                existingComment.State = entity.State;

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} Update method error", typeof(CommentRepository));
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
                _logger.LogError(ex, "{Repo} Delete method error", typeof(CommentRepository));
                return false;
            }
        }
    }
}