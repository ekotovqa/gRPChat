using Blazor.CssGrid.Enums;
using System.Text;

namespace Blazor.CssGrid.Helpers
{
    internal class GridChildGenerator
    {
        public static void GenerateStyle(ref string style, string height, string width, int row, int column, int rowSpan, int columnSpan, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            style += $"height:{height};";
            style += $"width:{width};";
            style += $"grid-column:{GetColumnOrRow(column, columnSpan)};";
            style += $"grid-row:{GetColumnOrRow(row, rowSpan)};";

            var horizontal = horizontalAlignment switch
            {
                HorizontalAlignment.Stretch => "stretch",
                HorizontalAlignment.Center => "centre",
                HorizontalAlignment.Left => "start",
                HorizontalAlignment.Right => "end",
                _ => ""
            };

            var vertical = verticalAlignment switch
            {
                VerticalAlignment.Stretch => "stretch",
                VerticalAlignment.Center => "centre",
                VerticalAlignment.Top => "start",
                VerticalAlignment.Bottom => "end",
                _ => ""

            };

            style += $"align-self:{vertical};";
            style += $"justify-self:{horizontal};";
        }

        #region Private Methods

        private static object GetColumnOrRow(int columnOrRow, int columnOrRowSpan)
        {
            var sb = new StringBuilder();

            sb.Append(columnOrRow > 1 ? columnOrRow : 1);
            sb.Append("/span ");
            sb.Append(columnOrRowSpan > 1 ? columnOrRowSpan : 1);

            return sb.ToString();
        }

        #endregion
    }
}
