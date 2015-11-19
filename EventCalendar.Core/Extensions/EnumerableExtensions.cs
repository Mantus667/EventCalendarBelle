using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCalendar.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool AnySave<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) return false;
            if (predicate == null) throw new ArgumentNullException("predicate");
            return source.Any(predicate);
        }

        public static bool AnySave<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) return false;
            return source.Any();
        }
    }
}
