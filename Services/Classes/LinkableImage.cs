using DataAccess.Models;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class LinkableImage
    {
        public Image Image { get; set; }
        public Link Link { get; set; }


        public async Task SetData(NicheShackContext context)
        {
            await Image.SetData(context);
            //if (Link != null) await Link.SetData(context);
        }
    }
}