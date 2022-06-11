﻿using DataAccess.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Classes
{
    public struct Row
    {
        public float Top { get; set; }
        public Background Background { get; set; }
        public Border Border { get; set; }
        public Corners Corners { get; set; }
        public Shadow Shadow { get; set; }
        public Padding Padding { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public List<Column> Columns { get; set; }




        public async Task<HtmlNode> Create(HtmlNode table, NicheShackContext context)
        {
            // Insert a row for spacing
            if (Top > 0)
            {
                HtmlNode blankRow = table.AppendChild(HtmlNode.CreateNode("<tr>"));
                HtmlNode blankColumn = blankRow.AppendChild(HtmlNode.CreateNode("<td>"));
                blankColumn.SetAttributeValue("height", Top.ToString());
            }

            // Create the row
            HtmlNode row = table.AppendChild(HtmlNode.CreateNode("<tr>"));

            // Set the styles
            if (Background != null) await Background.SetStyle(row, context);
            if (Border != null) Border.SetStyle(row);
            if (Corners != null) Corners.SetStyle(row);
            if (Shadow != null) Shadow.SetStyle(row);
            if (Padding != null) Padding.SetStyle(row);

            string valign = "top";

            if (VerticalAlignment != null && VerticalAlignment.Values.Count > 0)
            {
                switch (VerticalAlignment.Values[0].VerticalAlignmentType)
                {
                    case 1:
                        valign = "middle";
                        break;

                    case 2:
                        valign = "bottom";
                        break;

                    default:
                        valign = "top";
                        break;
                }
            }


            row.SetAttributeValue("valign", valign);

            return row;
        }
    }
}
