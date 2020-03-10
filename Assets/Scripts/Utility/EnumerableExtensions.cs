using System;
using System.Collections.Generic;

namespace Utility
{
    public static class EnumerableExtensions
    {
        // defines Zip method for three enumerables
        // modified from: https://stackoverflow.com/a/10297160
        public static IEnumerable<TResult> ZipThree<T1, T2, T3, TResult>(
            this IEnumerable<T1> source,
            IEnumerable<T2> second,
            IEnumerable<T3> third,
            Func<T1, T2, T3, TResult> func)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (second is null) throw new ArgumentNullException(nameof(second));
            if (third is null) throw new ArgumentNullException(nameof(third));
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (var e1 = source.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            using (var e3 = third.GetEnumerator())
                while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
                    yield return func(e1.Current, e2.Current, e3.Current);
        }
    }
}