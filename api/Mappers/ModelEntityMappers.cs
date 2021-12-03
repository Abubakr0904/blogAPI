using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using api.Core.IConfiguration;
using api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Mappers
{
    public static class ModelEntityMappers
    {
        public static Entities.Media GetMediaEntity(this IFormFile media)
        {
            using var stream = new MemoryStream();

            media.CopyTo(stream);
            return new Entities.Media(
                contentType: media.ContentType,
                data: stream.ToArray()
            );
        }

        public static Entities.Post GetPostEntity(this Models.Post post, List<Entities.Media> medias)
        {
            
            return new Entities.Post()
            {
                Id = Guid.NewGuid(),
                HeaderImageId = post.HeaderImageId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                CreatedAt = DateTimeOffset.UtcNow,
                ModifiedAt = DateTimeOffset.UtcNow,
                Comments = new List<Comment>(),
                Medias = medias
            };
        }

        public static Entities.Comment GetCommentEntity(this Models.Comment comment)
        {
            return new Entities.Comment()
            {
                Id = Guid.NewGuid(),
                Author = comment.Author,
                Content = comment.Content,
                State = comment.State.ToEntityStateEnumMapper(),
                PostId = comment.PostId
            };
        }

        // public static System.Guid ToEntityGuid(this System.Guid? id)
        // {
        //     if(id == null)
        //     {
        //         return default(Guid);
        //     }
        //     else
        //     {
        //         return id.Value;
        //     }
        // }
    }
}