using System.Collections.Generic;
using System.Linq;

namespace DataTypes
{
    public class Frequencies
    {
        private int[] _frequencies { get; }
        // counter for ticks since start
        private int _ticks { get; set; }

        public Frequencies(int[] frequencies)
        {
            _frequencies = frequencies;
        }

        public IEnumerable<int> CurrentActiveIndices()
        {
            var oldTicks = _ticks++;
            return _frequencies.Select((frequency, index) => new {frequency, index}).Where(f => oldTicks % f.frequency == 0).Select(f => f.index);
        }
    }
}