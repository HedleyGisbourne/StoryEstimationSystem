using System;

namespace HG.SoftwareEstimationService.Shared
{
    public class ColumnWidthAttribute : Attribute
    {
        public ColumnWidthAttribute(int width)
        {
            Width = width;
        }

        public int Width { get; set; }
    }
}
