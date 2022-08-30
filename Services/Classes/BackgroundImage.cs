namespace Services.Classes
{
    public class BackgroundImage : Image
    {
        public KeyValue Position { get; set; }
        public KeyValue Repeat { get; set; }
        public KeyValue Attachment { get; set; }


        public void SetStyle(ref string styles)
        {
            if (Position.Value != null) styles += "background-position: " + Position.Value + ";";
            if (Repeat.Value != null) styles += "background-repeat: " + Repeat.Value + ";";
            if (Attachment.Value != null) styles += "background-attachment: " + Attachment.Value + ";";
        }
    }
}
