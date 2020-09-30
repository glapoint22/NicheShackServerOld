using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services.Classes
{
    public class WidgetJsonConverter : JsonConverter<Widget>
    {
        public override Widget Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Widget widget = null;
            int startDepth = reader.CurrentDepth;


            while (reader.Read())
            {
                // Return the widget when we are at the end of the object
                if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == startDepth)
                {
                    return widget;
                }



                // If we are reading a property name
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string property = reader.GetString();

                    // If the property name is widget type
                    if (property == "widgetType")
                    {
                        reader.Read();
                        WidgetType widgetType = (WidgetType)reader.GetInt32();

                        // Get the widget
                        switch (widgetType)
                        {
                            case WidgetType.Button:
                                widget = new ButtonWidget();
                                break;
                            case WidgetType.Text:
                                widget = new TextWidget();
                                break;
                            case WidgetType.Image:
                                widget = new ImageWidget();
                                break;
                            case WidgetType.Container:
                                widget = new ContainerWidget();
                                break;
                            case WidgetType.Line:
                                widget = new LineWidget();
                                break;
                        }

                        widget.WidgetType = widgetType;
                    }
                    else
                    {
                        // Set each property for the widget
                        widget.SetProperty(property, ref reader, options);
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Widget value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
