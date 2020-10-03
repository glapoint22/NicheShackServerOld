namespace Services.Classes
{
    public class BackgroundImage : Image
    {
        public string Position { get; set; }
        public string Repeat { get; set; }
        public string Attachment { get; set; }


        public void SetStyle(ref string styles)
        {
            if (Position != null) styles += "background-position: " + Position + ";";
            if (Repeat != null) styles += "background-repeat: " + Repeat + ";";
            if (Attachment != null) styles += "background-attachment: " + Attachment + ";";
        }
    }
}
