using System.Collections.Generic;
using IconMaker.Model.ColorFilters;

namespace IconMaker.Model
{
    public class ColorMapEntry
    {
        public static List<IColorFilter> Filters = new List<IColorFilter>
        {
            new HSVShift(),
            new HSLShift(),
            new HSIShift(),
            new HCL601Shift(),
            new HCL709Shift(),
        };

        public ColorMapEntry(ColorEx originalColor)
        {
            OriginalColor = originalColor;
            ModifiedColor = originalColor;
        }

        public ColorEx OriginalColor { get; }
        public ColorEx ModifiedColor { get; }
    }
}
