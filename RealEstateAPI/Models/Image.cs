// Models/Image.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateAPI.Models
{
    public class Image
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Url { get; set; }

        [Required]
        public Guid PropertyId { get; set; }

        [ForeignKey("PropertyId")]
        public Property Property { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: Add image metadata
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
    }
}