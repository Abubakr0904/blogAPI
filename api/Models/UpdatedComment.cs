using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api.Models
{
    public class UpdatedComment
    {
        public string Content { get; set; }
        
        [JsonConverter(typeof(EnumConverter))]
        public EState? State { get; set; }
    }
}