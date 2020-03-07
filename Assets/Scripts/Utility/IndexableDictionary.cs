using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public class IndexableDictionary<TItem> : IEnumerable<TItem>
        where TItem : IComparable<TItem>
    {
        private Dictionary<TItem, int> _indices { get; } = new Dictionary<TItem, int>();
        private List<TItem> _values { get; } = new List<TItem>();
        // whether _indices is up to date
        private bool _integrity { get; set; } = true;

        public IEnumerator<TItem> GetEnumerator() => _values.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public void Add(TItem item)
        {
            _values.Add(item);
            _indices.Add(item, _values.Count - 1);
        }
        
        // harms data integrity
        public void UnsafeRemove(TItem item)
        {
            _indices.Remove(item);
            _values.Remove(item);
            // removing an element harms data integrity until Sort() is called again
            _integrity = false;
        }

        // remove an element without harming data integrity
        public void SafeRemove(TItem item)
        {
            UnsafeRemove(item);
            Sort();
        }

        public int GetIndex(TItem item)
        {
            if (! _integrity)
                throw new Exception("IndexableDictionary integrity is not guaranteed!");
            return _indices[item];
        }

        public TItem GetItem(TItem item) => GetItemAt(GetIndex(item));

        public TItem GetItemAt(int index) => _values[index];

        public IEnumerable<TItem> LookForward(TItem item) => LookForwardAt(GetIndex(item));

        public IEnumerable<TItem> LookForwardAt(int index) => _values.Skip(index + 1);
        
        public IEnumerable<TItem> LookBackward(TItem item) => LookBackwardAt(GetIndex(item));

        public IEnumerable<TItem> LookBackwardAt(int index)
        {
            for (var i = index - 1; i >= 0; i--)
                yield return _values[i];
        }

        public (IEnumerable<TItem> backward, IEnumerable<TItem> forward) LookAround(TItem item)
            => LookAroundAt(GetIndex(item));

        public (IEnumerable<TItem> backward, IEnumerable<TItem> forward) LookAroundAt(int index)
            => (LookBackwardAt(index), LookForwardAt(index));

        public void Sort()
        {
            QuickSort(0, _values.Count - 1);
            _integrity = true;
            
            void QuickSort(int leftIndex, int rightIndex)
            {
                if (leftIndex >= rightIndex) return;
                
                var pivotIndex = Partition(leftIndex, rightIndex);
                
                // update _indices since the item at pivotIndex is at its final position
                _indices[_values[pivotIndex]] = pivotIndex;
                
                QuickSort(leftIndex, pivotIndex - 1);
                QuickSort(pivotIndex + 1, rightIndex);
            }
            
            int Partition(int leftIndex, int rightIndex)
            {
                var i = leftIndex;
                var j = rightIndex - 1;
                var pivot = _values[(leftIndex + rightIndex) / 2];

                while (true) {
                    // search to the left for an element that's larger than the pivot
                    while (_values[i].CompareTo(pivot) <= 0 && i < rightIndex) 
                        i++;

                    // search to the right for an element that's smaller than the pivot
                    while (_values[j].CompareTo(pivot) >= 0 && j > leftIndex) 
                        j--;

                    if (i >= j) break;
                    
                    _values.SwapAt(i, j);
                }

                _values.SwapAt(i, rightIndex);

                return i;
            }
        }
    }
}