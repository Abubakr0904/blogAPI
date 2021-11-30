using api.Core.IConfiguration;
using api.Core.IRepositories;
using api.Core.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace api.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BlogDbContext _context;
        private readonly ILogger _logger;

        public IPostRepository Posts { get; private set; }
        public IMediaRepository Medias { get; private set; }
        public ICommentRepository Comments { get; private set; }

        public UnitOfWork(BlogDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger= loggerFactory.CreateLogger("logs");

            Posts = new PostRepository(_context, _logger);
            Medias = new MediaRepository(_context, _logger);
            Comments = new CommentRepository(_context, _logger);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}