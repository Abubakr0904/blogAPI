using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace api.Entities
{
    public class Comment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        
        [MaxLength(255)]
        [Required]
        public string Author { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public EState State { get; set; }

        [Required]
        public Guid PostId { get; set; }
    }
}