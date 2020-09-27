using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Services.Classes
{
    public class TextWidget : Widget
    {
        public override HtmlNode Create(HtmlNode column)
        {
            throw new NotImplementedException();
        }

        public override void SetProperty(Utf8JsonReader reader, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
