using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Models
{
    public class Comment
    {
        [MaxLength(255)]
        [Required]
        public string Author { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EState? State { get; set; }

        [Required]
        public Guid PostId { get; set; }
    }
}