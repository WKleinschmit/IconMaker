using System.Collections.ObjectModel;
using System.Linq;

namespace IconMaker.Model
{
    public class ColorMap : ObservableCollection<ColorMapEntry>
    {
        public bool TryGetValue(ColorEx colorEx, out ColorMapEntry colorMapEntry)
        {
            colorMapEntry = this.FirstOrDefault(e => e.OriginalColor == colorEx);
            return colorMapEntry != null;
        }
    }
}