using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace api.Models
{
    public class Media
    {
        [Required]
        public IEnumerable<IFormFile> Files { get; set; }
    }
}