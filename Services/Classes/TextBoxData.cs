using System.Collections.Generic;

namespace Services.Classes
{
    public class TextBoxData
    {
        public ElementType ElementType { get; set; }
        public List<TextBoxData> Children { get; set; }
        public string Text { get; set; }
        public List<StyleData> Styles { get; set; }
        public Link Link { get; set; }
        public int? Indent { get; set; }
    }
}