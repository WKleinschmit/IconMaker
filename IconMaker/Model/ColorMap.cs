namespace IconMaker.Model
{
    public class ColorMap : IntrusiveDictionary<ColorEx, ColorMapEntry>
    {
        public ColorMap()
            : base(v => v.OriginalColor)
        {

        }

    }
}