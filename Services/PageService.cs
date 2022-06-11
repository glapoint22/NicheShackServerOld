using DataAccess.Models;
using Services.Classes;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class PageService : IPageService
    {
        private readonly NicheShackContext context;
        private PageContent page;

        public PageService(NicheShackContext context)
        {
            this.context = context;
        }

        public async Task<PageContent> GePage(string pageContent, QueryParams queryParams)
        {
            // Deserialize the content into a page
            page = JsonSerializer.Deserialize<PageContent>(pageContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // If background has an image
            if (page.Background != null && page.Background.Image != null)
            {
                await page.Background.Image.SetData(context);
            }


            // Rows
            if (page.Rows != null && page.Rows.Count() > 0)
            {
                await SetData(page.Rows, context, queryParams);
            }


            return page;
        }




        private async Task SetData(IEnumerable<Row> rows, NicheShackContext context, QueryParams queryParams)
        {
            foreach (Row row in rows)
            {
                if (row.Background != null && row.Background.Image != null)
                {
                    await row.Background.Image.SetData(context);
                }

                foreach (Column column in row.Columns)
                {
                    if (column.Background != null && column.Background.Image != null)
                    {
                        await column.Background.Image.SetData(context);
                    }


                    // Create the widget
                    Widget widget = page.GetWidget(column.WidgetData.WidgetType, column.WidgetData);


                    await widget.SetData(context, queryParams);

                    if (column.WidgetData.WidgetType == WidgetType.Container)
                    {
                        ContainerWidget container = (ContainerWidget)column.WidgetData;

                        if (container.Rows != null && container.Rows.Count() > 0) await SetData(container.Rows, context, queryParams);
                    }
                }
            }

        }


    }
}
