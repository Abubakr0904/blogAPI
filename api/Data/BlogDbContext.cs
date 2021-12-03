using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class BlogDbContext : DbContext
    {
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        
        public BlogDbContext(DbContextOptions options)
            :base(options) {  }
    }
}