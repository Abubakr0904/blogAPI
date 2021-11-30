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
    public class MediaRepository: GenericRepository<Media>, IMediaRepository
    {
        public MediaRepository(BlogDbContext context, ILogger logger)
            :base(context, logger)
        {
            
        }
        public override async Task<IEnumerable<Media>> GetAll()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method error", typeof(MediaRepository));
                return new List<Media>();
            }
        }

        public override async Task<Media> GetById(Guid id)
        {
            try
            {
                return await _dbSet
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetById method error", typeof(PostRepository));
                return default(Media);
            }
        }

        public override async Task<bool> Add(Media entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Add method error", typeof(MediaRepository));
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
                _logger.LogError(ex, "{Repo} Delete method error", typeof(MediaRepository));
                return false;
            }
        }
    }
}