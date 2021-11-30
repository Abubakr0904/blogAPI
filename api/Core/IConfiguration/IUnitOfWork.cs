using System.Threading.Tasks;
using api.Core.IRepositories;

namespace api.Core.IConfiguration
{
    public interface IUnitOfWork
    {
        IPostRepository Posts {get; }
        ICommentRepository Comments {get; }
        IMediaRepository Medias {get; }

        Task CompleteAsync();
    }
}