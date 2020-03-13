using System.Collections.Generic;

namespace Utility
{
    public static class ListExtensions
    {
        // returns specified element in list and removes it
        // modified from: https://stackoverflow.com/a/24855920
        public static T PopAt<T>(this IList<T> list, int index)
        {
            var r = list[index];
            list.RemoveAt(index);
            return r;
        }

        public static void SwapAt<T>(this IList<T> list, int i1, int i2)
        {
            var copy = list[i1];
            list[i1] = list[i2];
            list[i2] = copy;
        }
    }
}