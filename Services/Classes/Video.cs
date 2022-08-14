using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class Video
    {
        public int Id { get; set; }
        public string Thumbnail { get; set; }
        public string Name { get; set; }
        public int VideoType { get; set; }
        public string VideoId { get; set; }

        public async Task SetData(NicheShackContext context)
        {
            Video video = await context.Media
            .AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new Video
            {
                Name = x.Name,
                Thumbnail = x.Thumbnail,
                VideoType = x.VideoType,
                VideoId = x.VideoId
            })
            .SingleOrDefaultAsync();


            // If this video exists
            if (video != null)
            {
                Name = video.Name;
                Thumbnail = video.Thumbnail;
                VideoType = video.VideoType;
                VideoId = video.VideoId;
            }
        }
    }
}
