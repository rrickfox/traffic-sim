using System.Linq;
using System.Collections.Generic;

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

        // modified from: https://stackoverflow.com/a/43345968
        public static List<double> CalculateCumulative(int[] weights)
        {
            var sumWeights = (double) weights.Sum();
            // generate distribution probability
            var distribution = weights.Select(v => v / sumWeights).ToArray();

            var sum = 0d;
            // first change shape of your distribution probability array
            // we need it to be cumulative, that is:
            // if you have [0.1, 0.2, 0.3, 0.4] 
            // we need     [0.1, 0.3, 0.6, 1  ] instead
            return distribution.Select(c => {
                var result = c + sum;
                sum += c;
                return result;
            }).ToList();
        }

        public static float Square(float value) => value * value;
    }
}