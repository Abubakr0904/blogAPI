using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace api.Entities
{
    public class Media
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [MaxLength(55)]
        public string ContentType { get; set; }

        [MaxLength(3 * 1024 * 1024)]
        public byte[] Data { get; set; }
        
        public Media(string contentType, byte[] data)
        {
            Id = Guid.NewGuid();
            ContentType = contentType;
            Data = data;
        }
    }
}