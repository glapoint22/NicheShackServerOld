namespace Manager.Classes
{
    public class SearchedMedia
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }


        public string ImageSm { get; set; }
        public int ImageSmWidth { get; set; }
        public int ImageSmHeight { get; set; }


        public string ImageMd { get; set; }
        public int ImageMdWidth { get; set; }
        public int ImageMdHeight { get; set; }


        public string ImageLg { get; set; }
        public int ImageLgWidth { get; set; }
        public int ImageLgHeight { get; set; }



        public string ImageAnySize { get; set; }
        public int ImageAnySizeWidth { get; set; }
        public int ImageAnySizeHeight { get; set; }
    }
}
