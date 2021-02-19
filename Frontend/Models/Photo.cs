using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class Photo
    {
        public string Id { get; set; }
        public DateTime UploadDate { get; set; }
        public string ThumbnailImageData { get; set; }
        public string ProcessedImageData { get; set; }
        public bool IsProcessed { get; set; }
    }
}
