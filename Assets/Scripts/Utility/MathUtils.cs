using System.Linq;

namespace Utility
{
    public static class MathUtils
    {
        // https://stackoverflow.com/a/13493771
        // returns number of decimal places
        public static int DecimalPlaces(float value)
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture)
                .TrimEnd('0')
                .SkipWhile(c => c != '.')
                .Skip(1)
                .Count();
        }
        public static float Square(float value)
        {
            return (value * value);
        }
    }
}