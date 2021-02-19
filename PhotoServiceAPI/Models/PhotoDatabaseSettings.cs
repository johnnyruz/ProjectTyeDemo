using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoServiceAPI.Models
{
    public class PhotosDatabaseSettings : IPhotosDatabaseSettings
    {
        public string PhotosCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IPhotosDatabaseSettings
    {
        string PhotosCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}