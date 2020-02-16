using System.Collections.Generic;

namespace Utility
{
    public static class ListExtensions
    {
        // returns first element in list and removes it
        // modified from: https://stackoverflow.com/a/24855920
        public static T PopAt<T>(this IList<T> list, int index)
        {
            var r = list[index];
            list.RemoveAt(index);
            return r;
        }
    }
}