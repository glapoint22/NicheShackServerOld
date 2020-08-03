using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.ViewModels
{
    public class MediaViewModel : ImageViewModel
    {
        public string Thumbnail { get; set; }
        public MediaType Type { get; set; }
    }



    public enum MediaType
    {
        Image,
        BackgroundImage,
        BannerImage,
        CategoryImage,
        ProductImage,
        Icon,
        Video,
        Search,
        ProductMediaImage
    }
}
