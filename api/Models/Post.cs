using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Post
    {   
        public Guid HeaderImageId { get; set; } 

        [Required]
        [MaxLength(255)]        
        public string Title { get; set; }
        
        [MaxLength(255)]
        public string Description { get; set; }
        
        public string Content { get; set; }
        
        public IEnumerable<Guid> MediaIds { get; set; }
    }
}