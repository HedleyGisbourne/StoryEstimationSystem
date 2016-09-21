using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.SoftwareEstimationService.Helpers
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Safe<T>(this IEnumerable<T> obj)
        {
            return obj ?? Enumerable.Empty<T>();
        }

        public static TS[] SelectArray<T, TS>(this IEnumerable<T> obj, Func<T, TS> func)
        {
            return obj.Select(func).ToArray();
        }

        public static IList<TS> SelectList<T, TS>(this IEnumerable<T> obj, Func<T, TS> func)
        {
            return obj.Select(func).ToList();
        }

        public static T[] WhereArray<T>(this IEnumerable<T> obj, Func<T, bool> func)
        {
            return obj.Where(func).ToArray();
        }

        public static IList<T> WhereList<T>(this IEnumerable<T> obj, Func<T, bool> func)
        {
            return obj.Where(func).ToList();
        }

        // http://stackoverflow.com/questions/457453/remove-element-of-a-regular-array
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
    }
}