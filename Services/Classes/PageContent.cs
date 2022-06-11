using System.Collections.Generic;

namespace Services.Classes
{
    public class PageContent
    {
        public int Width { get; set; }
        public Background Background { get; set; }
        public IEnumerable<Row> Rows { get; set; }


        public Widget GetWidget(WidgetType widgetType, Widget widgetData)
        {
            Widget widget = null;

            switch (widgetType)
            {
                case WidgetType.Button:
                    widget = (ButtonWidget)widgetData;
                    break;
                case WidgetType.Text:
                    widget = (TextWidget)widgetData;
                    break;
                case WidgetType.Image:
                    widget = (ImageWidget)widgetData;
                    break;
                case WidgetType.Container:
                    widget = (ContainerWidget)widgetData;
                    break;
                case WidgetType.Line:
                    widget = (LineWidget)widgetData;
                    break;
                case WidgetType.Video:
                    widget = (VideoWidget)widgetData;
                    break;
                case WidgetType.ProductGroup:
                    widget = (ProductGroupWidget)widgetData;
                    break;
                case WidgetType.Shop:
                    widget = (ShopWidget)widgetData;
                    break;
                case WidgetType.Carousel:
                    widget = (CarouselWidget)widgetData;
                    break;
                case WidgetType.Grid:
                    widget = (GridWidget)widgetData;
                    break;
                case WidgetType.Section:
                    widget = (SectionWidget)widgetData;
                    break;
                case WidgetType.Divider:
                    widget = (DividerWidget)widgetData;
                    break;
            }

            return widget;
        }
    }
}
