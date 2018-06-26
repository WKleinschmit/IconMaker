using System.Windows.Media;

namespace IconMaker.Model.ColorFilters
{
    public interface IColorFilter
    {
        string Title { get; }
        ColorEx Convert(ColorEx input);
    }
}
