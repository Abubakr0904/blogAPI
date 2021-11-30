using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.Core.IConfiguration;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
//using api.Models;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly ILogger<MediaController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public MediaController(ILogger<MediaController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // [HttpGet("")]
        // public ActionResult<IEnumerable<TModel>> GetTModels()
        // {
        //     return new List<TModel> { };
        // }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetMedia([FromRoute]Guid id)
        {
            var file = await _unitOfWork.Medias.GetById(id);

            var stream = new MemoryStream(file.Data);

            return File(stream, file.ContentType);
        }

        [HttpGet]
        public async Task<IActionResult> GetMedias()
        {
            var medias = await _unitOfWork.Medias.GetAll();
            var result = medias.Select(u => new {
                Id = u.Id,
                ContentType = u.ContentType,
                });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> PostMedia([FromForm]Models.Media medias)
        {
            if(ModelState.IsValid)
            {
                var mFiles = medias.Files.Select(m => m.GetMediaEntity()).ToList();

                mFiles.ForEach(m => _unitOfWork.Medias.Add(m));

                await _unitOfWork.CompleteAsync();

                return Ok(mFiles.Select(m => new { 
                    Id = m.Id,
                    ContentType = m.ContentType
                    }).ToList());
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }

        // [HttpPut("{id}")]
        // public IActionResult PutTModel(int id, TModel model)
        // {
        //     return NoContent();
        // }

        // [HttpDelete("{id}")]
        // public ActionResult<TModel> DeleteTModelById(int id)
        // {
        //     return null;
        // }
    }
}