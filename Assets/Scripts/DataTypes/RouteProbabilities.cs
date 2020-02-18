using System.Linq;
using System.Collections.Generic;
using System;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class RouteProbabilities
    {
        private int[] _weights;
        private List<IVertex> _destinations;
        private List<double> _cumulative;

        public RouteProbabilities(int[] weights, List<IVertex> destinations)
        {
            _weights = weights;
            _destinations = destinations;
            
            CalculateCumulative();
        }

        // modified from: https://stackoverflow.com/a/43345968
        private void CalculateCumulative()
        {
            var sumWeights = (double) _weights.Sum();
            // generate distribution probability
            var distribution = _weights.Select(v => v / sumWeights).ToArray();

            var sum = 0d;
            // first change shape of your distribution probability array
            // we need it to be cumulative, that is:
            // if you have [0.1, 0.2, 0.3, 0.4] 
            // we need     [0.1, 0.3, 0.6, 1  ] instead
            _cumulative = distribution.Select(c => {
                var result = c + sum;
                sum += c;
                return result;
            }).ToList();
        }

        // selects an IVertex from destinations based on weights
        // modified from: https://stackoverflow.com/a/43345968
        public IVertex Choose()
        {
            // now generate random double. It will always be in range from 0 to 1
            var r = RANDOM.NextDouble();
            // now find first index in our cumulative array that is greater or equal generated random value
            var idx = _cumulative.BinarySearch(r);
            // if exact match is not found, List.BinarySearch will return index of the first items greater than passed value, but in specific form (negative)
            // we need to apply ~ to this negative value to get real index
            if (idx < 0)
                idx = ~idx; 
            if (idx > _cumulative.Count - 1)
                idx = _cumulative.Count - 1; // very rare case when probabilities do not sum to 1 because of double precision issues (so sum is 0.999943 and so on)
            // return item at given index
            return _destinations[idx];
        }
    }
}