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

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteMedia([FromRoute]Guid id)
        {
            try
            {
                var result = await _unitOfWork.Medias.Delete(id);

                if(result)
                {
                    await _unitOfWork.CompleteAsync();
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(new Exception($"Something went wrong: {e.Message}"));
            }

        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetMedia([FromRoute]Guid id)
        {
            var file = await _unitOfWork.Medias.GetById(id);
            if(file != default)
            {
                var stream = new MemoryStream(file.Data);
                return File(stream, file.ContentType);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetMedias()
        {
            var medias = await _unitOfWork.Medias.GetAll();
            
            if(medias != new List<Entities.Media>())
            {
                var result = medias.Select(u => new {
                    Id = u.Id,
                    ContentType = u.ContentType,
                    });
                return Ok(result);
            }

            return NotFound();
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
    }
}