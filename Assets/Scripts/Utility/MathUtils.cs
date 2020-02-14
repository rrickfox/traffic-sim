using System.Linq;
using System.Collections.Generic;
using System;

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

        // selects an element from an IList based on a weight
        // modified from: https://stackoverflow.com/a/43345968
        static T Choice<T>(IList<T> sequence, int[] weights)
        {
            double[] distribution = weights.Select(v => v / (double) weights.Sum()).ToArray();
            var random = new Random();
            double sum = 0;
            // first change shape of your distribution probability array
            // we need it to be cumulative, that is:
            // if you have [0.1, 0.2, 0.3, 0.4] 
            // we need     [0.1, 0.3, 0.6, 1  ] instead
            var cumulative = distribution.Select(c => {
                var result = c + sum;
                sum += c;
                return result;
            }).ToList();
            // now generate random double. It will always be in range from 0 to 1
            var r = random.NextDouble();
            // now find first index in our cumulative array that is greater or equal generated random value
            var idx = cumulative.BinarySearch(r);
            // if exact match is not found, List.BinarySearch will return index of the first items greater than passed value, but in specific form (negative)
            // we need to apply ~ to this negative value to get real index
            if (idx < 0)
                idx = ~idx; 
            if (idx > cumulative.Count - 1)
                idx = cumulative.Count - 1; // very rare case when probabilities do not sum to 1 because of double precision issues (so sum is 0.999943 and so on)
            // return item at given index
            return sequence[idx];
        }
    }
}