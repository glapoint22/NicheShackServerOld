namespace Services.Classes
{
    public class BackgroundImage : Image
    {
        public KeyValue Position { get; set; }
        public KeyValue Repeat { get; set; }
        public KeyValue Attachment { get; set; }


        public void SetStyle(ref string styles)
        {
            //if (Position != null) styles += "background-position: " + Position + ";";
            //if (Repeat != null) styles += "background-repeat: " + Repeat + ";";
            //if (Attachment != null) styles += "background-attachment: " + Attachment + ";";
        }
    }
}
