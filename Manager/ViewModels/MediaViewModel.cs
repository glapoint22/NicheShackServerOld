using DataAccess.Interfaces;
using DataAccess.Models;
using System.Linq;

namespace Manager.ViewModels
{
    public class MediaViewModel : ImageViewModel, IQueryableSelect<Media, MediaViewModel>, IItem
    {
        public string Thumbnail { get; set; }
        public string ImageAnySize { get; set; }
        public string Image200 { get; set; }
        public string Image500 { get; set; }
        public string Image675 { get; set; }
        public int Type { get; set; }
        public string VideoId { get; set; }
        public int VideoType { get; set; }


        public IQueryable<MediaViewModel> Select(IQueryable<Media> source)
        {
            return source.Select(x => new MediaViewModel
            {
                Id = x.Id,
                Name = x.Name,
                ImageAnySize = x.ImageAnySize,
                Thumbnail = x.Thumbnail,
                Image200 = x.ImageSm,
                Image500 = x.ImageMd,
                Image675 = x.ImageLg,
                VideoId = x.VideoId,
                VideoType = x.VideoType
            });
        }
    }
}
