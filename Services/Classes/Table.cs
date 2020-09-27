using HtmlAgilityPack;

namespace Services.Classes
{
    public class Table
    {

        public static HtmlNode Create(HtmlNode parent)
        {
            HtmlNode table = CreateTable(parent);
            table.SetAttributeValue("style", "width: 100%;");
            table.SetAttributeValue("width", "100%");

            return table;
        }


        public static HtmlNode Create(HtmlNode parent, float? width = null, float? height = null)
        {
            string styles = "";

            HtmlNode table = CreateTable(parent);

            // Width
            if (width != null && width > 0)
            {
                parent.InsertBefore(new HtmlDocument().CreateComment("<!--[if (gte mso 9)|(IE)]><table width=\"" + width + "\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td>[endif]-->"), table);
                parent.AppendChild(new HtmlDocument().CreateComment("<!--[if (gte mso 9)|(IE)]></td></tr></table>[endif]-->"));

                styles += "width: 100%;max-width: " + width + "px;";
            }
            else
            {
                styles += "width: 100%;";
                table.SetAttributeValue("width", "100%");
            }



            // Height
            if (height != null && height > 0)
            {
                styles += "height: " + height + "px;";
                table.SetAttributeValue("height", height.ToString());
            }


            table.SetAttributeValue("style", styles);

            return table;
        }




        private static HtmlNode CreateTable(HtmlNode parent)
        {
            HtmlNode table = parent.AppendChild(HtmlNode.CreateNode("<table>"));
            table.SetAttributeValue("width", "100%");
            table.SetAttributeValue("cellPadding", "0");
            table.SetAttributeValue("cellSpacing", "0");
            table.SetAttributeValue("border", "0");

            return table;
        }

        public static void CreateRow(HtmlNode table)
        {
            HtmlNode tr = HtmlNode.CreateNode("<tr>");
            HtmlNode td = HtmlNode.CreateNode("<td>");

            tr.AppendChild(td);
            table.AppendChild(tr);
        }
    }
}
