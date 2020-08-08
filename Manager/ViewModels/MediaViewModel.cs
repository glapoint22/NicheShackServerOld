using static Manager.Classes.Utility;

namespace Manager.ViewModels
{
    public class MediaViewModel : ImageViewModel
    {
        public string Thumbnail { get; set; }
        public MediaType Type { get; set; }
    }
}
