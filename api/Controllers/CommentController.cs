using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Core.IConfiguration;
using api.Data;
using api.Entities;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using api.Models;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly BlogDbContext _ctx;

        public CommentController(ILogger<CommentController> logger, IUnitOfWork unitOfWork, BlogDbContext context)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _ctx = context;
        }

        // [HttpGet("")]
        // public ActionResult<IEnumerable<TModel>> GetTModels()
        // {
        //     return new List<TModel> { };
        // }

        [HttpGet("{PostId}")]
        public async Task<IActionResult> GetCommentsByPostId([FromRoute]Guid PostId)
        {
            var comments = await _unitOfWork.Comments.GetAllById(PostId);
            return Ok(comments);
        }

        [HttpPost("")]
        public async Task<IActionResult> PostComment(Models.Comment comment)
        {
            try
            {
                var entity = comment.GetCommentEntity();
                var postEntity = await _unitOfWork.Posts.GetById(comment.PostId);
                if(postEntity == default(Entities.Post))
                {
                    return BadRequest("Post Id is not valid or not matching the ones in the database.");
                }
                postEntity.Comments.Add(entity);
                _ctx.Posts.Update(postEntity);
                await _unitOfWork.CompleteAsync();
                return Ok(new {
                    Id = entity.Id,
                    Author = entity.Author,
                    Content = entity.Content,
                    PostId = entity.PostId,
                    State = entity.State
                });
            }
            catch (Exception e)
            {
                return BadRequest($"Something went wrong. {e.Message}");
            }
        }

        // [HttpPut("{id}")]
        // public IActionResult PutTModel(int id, TModel model)
        // {
        //     return NoContent();
        // }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentById(Guid id)
        {
            if(_unitOfWork.Comments.GetById(id).Result == default(Entities.Comment))
            {
                return NotFound("Comment with given id is not found.");
            }
            var result = await _unitOfWork.Comments.Delete(id);
            await _unitOfWork.CompleteAsync();
            if(result)
            {
                return NoContent();
            }
            return BadRequest("Something went wrong. See the repository error.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute]Guid id, [FromForm]Models.UpdatedComment comment)
        {
            var entity = await _unitOfWork.Comments.GetById(id);
            if(entity == default)
            {
                return NotFound("Comment with given id is not found.");
            }
            if(comment.State.ToEntityStateEnumMapper() == entity.State && entity.Content == comment.Content)
            {
                return BadRequest("You should change at least one property.");
            }
            var newComment = new Entities.Comment()
            {
                Id = entity.Id,
                Author = entity.Author,
                Content = comment.Content,
                State = comment.State.ToEntityStateEnumMapper(),
                PostId = entity.PostId
            };

            var result = await _unitOfWork.Comments.Add(newComment);
            await _unitOfWork.CompleteAsync();

            if(result)
            {
                return Ok(new {
                    Id = newComment.Id,
                    Author = newComment.Author,
                    State = newComment.State,
                    PostId = newComment.PostId
                });
            }
            else
            {
                return BadRequest("Something went wrong.");
            }
        }
    }
}