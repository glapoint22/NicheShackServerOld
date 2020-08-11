using DataAccess.Interfaces;
using DataAccess.Models;
using System.Linq;
using static Manager.Classes.Utility;

namespace Manager.ViewModels
{
    public class MediaViewModel : ImageViewModel, ISelect<Media, MediaViewModel>, IItem
    {
        public string Thumbnail { get; set; }
        public int Type { get; set; }


        public IQueryable<MediaViewModel> ViewModelSelect(IQueryable<Media> source)
        {
            return source.Select(x => new MediaViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Url = x.Url,
                Thumbnail = x.Thumbnail,
                Type = x.Type
            });
        }
    }
}
