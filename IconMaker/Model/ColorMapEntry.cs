using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconMaker.Model
{
    public class ColorMapEntry
    {
        public ColorMapEntry(ColorEx originalColor)
        {
            OriginalColor = originalColor;
            ModifiedColor = originalColor;
        }

        public ColorEx OriginalColor { get; }
        public ColorEx ModifiedColor { get; }
    }
}
