using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Core.IConfiguration;
using api.Entities;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using api.Models;
using api.Data;
//using api.Models;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly BlogDbContext _ctx;

        public PostController(ILogger<PostController> logger, IUnitOfWork unitOfWork, BlogDbContext context)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _ctx = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                 var entities = await _unitOfWork.Posts.GetAll();
                if(entities == default(List<Entities.Post>))
                {
                    return NotFound(new {
                        error = "Any post is not found.",
                        data = "null"
                    });
                }
                    // total = posts.Count(),
                    // posts
                var posts = entities.Select(p =>
                {
                    var comments = p.Comments == null ? default(List<Entities.Comment>) : p.Comments.Select(c => new Entities.Comment(){
                        Id = c.Id,
                        Author = c.Author,
                        Content = c.Content,
                        State = c.State,
                        PostId = c.PostId
                    });
                    var medias = p.Medias == null ? default(List<Entities.Media>) : p.Medias.Select(m => new Entities.Media(string.Empty, default(byte[])) {
                        Id = m.Id,
                        ContentType = m.ContentType
                    }).ToList();
                    return new {
                        id = p.Id,
                        headerImageUrl = p.HeaderImageId,
                        title = p.Title,
                        description = p.Description,
                        content = p.Content,
                        viewed = p.Viewed,
                        createdAt = p.CreatedAt,
                        modifiedAt = p.ModifiedAt,
                        comments = comments,
                        medias = medias
                    };
                });

                return Ok(new {
                    error = "",
                    data = new {
                        total = entities.Count(),
                        posts = posts
                    }
                });
            }
            catch (Exception e)
            {
                return BadRequest(new {StatusCode = 500, Error = e.Message});                
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            try
            {
                var p = await _unitOfWork.Posts.GetById(id);

                var comments = p.Comments == null ? default(List<Entities.Comment>) : p.Comments.Select(c => new Entities.Comment(){
                        Id = c.Id,
                        Author = c.Author,
                        Content = c.Content,
                        State = c.State,
                        PostId = c.PostId
                    }).ToList();
                    
                    var medias = p.Medias == null ? default(List<Entities.Media>) : p.Medias.Select(m => new Entities.Media(string.Empty, default(byte[])) {
                        Id = m.Id,
                        ContentType = m.ContentType
                    }).ToList();

                if(p == default(Entities.Post))
                {
                    return NotFound("No post with given id");
                }
                var post = new {
                        id = p.Id,
                        headerImageUrl = p.HeaderImageId,
                        title = p.Title,
                        description = p.Description,
                        content = p.Content,
                        viewed = p.Viewed,
                        createdAt = p.CreatedAt,
                        modifiedAt = p.ModifiedAt,
                        comments = comments,
                        medias = medias
                    };
                
                return Ok(post);
            }
            catch (Exception e)
            {
                return BadRequest($"Something went wrong. {e.Message}");               
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostPost([FromBody]Models.Post post)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var header = await _unitOfWork.Medias.GetById(post.HeaderImageId);
                    if(header == default(Entities.Media))
                    {
                        return BadRequest("The header image does not exist in Db.");
                    }

                    var mediaIds = post.MediaIds;
                    foreach (var mediaId in mediaIds)
                    {
                        var media = await _unitOfWork.Medias.GetById(mediaId);
                        if(media == default(Entities.Media))
                        {
                            return BadRequest("One or more of the images does not exist in Db.");
                        }
                    }

                    var medias = post.MediaIds.Select(id => _unitOfWork.Medias.GetById(id).Result).ToList();
                    var postEntity = post.GetPostEntity(medias);
                    await _unitOfWork.Posts.Add(postEntity);

                    await _unitOfWork.CompleteAsync();

                    return Ok(new {
                        Id = postEntity.Id,
                        HeaderImageId = postEntity.HeaderImageId,
                        Title = postEntity.Title,
                        MediaIds = postEntity.Medias.Select(m => m.Id).ToList(),
                    });
                }
                catch (Exception e)
                {
                    return BadRequest($"Something went wrong. {e.Message}");                    
                }
            }
            else
            {
                return BadRequest("Validation errors occur.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost([FromRoute]Guid id, [FromForm]UpdatedPost post)
        {
            if(post.MediaIds.GroupBy(x => x).Any(g => g.Count() > 1))
            {
                return BadRequest("There are duplicate image Ids.");
            }
            var entity = await _unitOfWork.Posts.GetById(id);
            if(entity == default(Entities.Post))
            {
                return NotFound("Entity with given id is not found.");
            }
            var entityMediaIds = entity.Medias
                .Select(i => i.Id)
                .ToList();

            post.MediaIds.ToList().ForEach(i => {
                entityMediaIds
                .Where(j => j != i && j != post.HeaderImageId)
                .ToList().ForEach(async k =>  {
                        await _unitOfWork.Medias.Delete(k);
                        await _unitOfWork.CompleteAsync();
                    });
            });
            var postEntity = new Entities.Post()
            {
                Id = id,
                HeaderImageId = post.HeaderImageId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Viewed = entity.Viewed,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = DateTimeOffset.UtcNow,
                Comments = entity.Comments,
                Medias = post.MediaIds.Select(m => _unitOfWork.Medias.GetById(m).Result).ToList()
            };

            await _unitOfWork.Posts.Update(postEntity);
            await _unitOfWork.CompleteAsync();


            return Ok(new {
                Id = postEntity.Id,
                HeaderImageId = postEntity.HeaderImageId,
                Title = postEntity.Title,
                Description = postEntity.Description,
                MediaIds = postEntity.Medias.Select(i => i.Id).ToList()
            });


            // var medias = new List<Entities.Media>();
            // var postEntity = await _unitOfWork.Posts.GetById(id);
            // if(postEntity == default(Entities.Post))
            // {
            //     return NotFound("Post with given id is not found.");
            // }
            // if(post.MediaIds == null)
            // {
            //     medias = postEntity.Medias.ToList();
            // }
            // else
            // {
            //     post.MediaIds.ToList().ForEach(m => {
            //         if(!postEntity.Medias.Contains(_unitOfWork.Medias.GetById(m).Result))
            //         {
            //             postEntity.Medias.Add(_unitOfWork.Medias.GetById(m).Result);
            //             _ctx.Posts.Update(postEntity);
            //             _unitOfWork.CompleteAsync();
            //         }
            //     });
            // }
            // var newPost = new Entities.Post()
            // {
            //     Id = postEntity.Id,
            //     HeaderImageId = post.HeaderImageId == null ? postEntity.HeaderImageId : post.HeaderImageId,
            //     Title = post.Title == null ? postEntity.Title : post.Title,
            //     Description = post.Description == null ? postEntity.Description : post.Description,
            //     Content = post.Content == null ? postEntity.Content : post.Content,
            //     Viewed = postEntity.Viewed,
            //     CreatedAt = postEntity.CreatedAt,
            //     ModifiedAt = DateTimeOffset.UtcNow,
            //     Comments = postEntity.Comments,
            //     Medias = postEntity.Medias
            // };

            // var result = await _unitOfWork.Posts.Update(postEntity);
            // await _unitOfWork.CompleteAsync();

            // if(result)
            // {
            //     return Ok(postEntity);
            // }
            // else
            // {
            //     return BadRequest("Something went wrong.");
            // }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostById(Guid id)
        {
            if(_unitOfWork.Posts.GetById(id).Result == default(Entities.Post))
            {
                return NotFound("Post with given id is not found");
            }

            var post = await _unitOfWork.Posts.GetById(id);

            await _unitOfWork.Medias.Delete(post.HeaderImageId);
            await _unitOfWork.CompleteAsync();
            
            post.Medias.ToList().ForEach(m => {
                _unitOfWork.Medias.Delete(m.Id);
                _unitOfWork.CompleteAsync();
                });
            
            
            var result = await _unitOfWork.Posts.Delete(id);
            await _unitOfWork.CompleteAsync();
            if(result)
            {
                return NoContent();
            }
            return BadRequest("Something went wrong. See the repository error.");
        }
    }
}