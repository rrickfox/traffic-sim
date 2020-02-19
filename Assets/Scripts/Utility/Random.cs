using DataTypes;
using System.Collections.Generic;

namespace Utility
{

    public static class Random
    {
        // global random Instance
        public static System.Random RANDOM = new System.Random();

        // selects an IVertex from destinations based on weights
        // modified from: https://stackoverflow.com/a/43345968
        public static IVertex Choose(List<double> cumulativeProbabilities, List<IVertex> destinations)
        {
            // now generate random double. It will always be in range from 0 to 1
            var r = RANDOM.NextDouble();
            // now find first index in our cumulative array that is greater or equal generated random value
            var idx = cumulativeProbabilities.BinarySearch(r);
            // if exact match is not found, List.BinarySearch will return index of the first items greater than passed value, but in specific form (negative)
            // we need to apply ~ to this negative value to get real index
            if (idx < 0)
                idx = ~idx; 
            if (idx > cumulativeProbabilities.Count - 1)
                idx = cumulativeProbabilities.Count - 1; // very rare case when probabilities do not sum to 1 because of double precision issues (so sum is 0.999943 and so on)
            // return item at given index
            return destinations[idx];
        }
    }
}