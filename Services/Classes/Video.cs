using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class Video
    {
        public int Id { get; set; }
        public string Src { get; set; }
        public string Thumbnail { get; set; }
        public string Name { get; set; }

        public async Task SetData(NicheShackContext context)
        {
            Video video = await context.Media
            .AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new Video
            {
                Thumbnail = x.Thumbnail
            })
            .SingleOrDefaultAsync();


            // If this video exists
            if (video != null)
            {
                Thumbnail = video.Thumbnail;
            }
        }
    }
}
