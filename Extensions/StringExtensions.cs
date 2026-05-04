using System.Collections.Generic;
using System.Linq;

namespace Perfecto.Deploy.Extensions
{
    public static class StringExtensions
    {
        public static string SJoin<T>(this IEnumerable<T> values, string delimiter = ", ") => string.Join(delimiter, values);
        public static string SJoinN<T>(this IEnumerable<T> values, string delimiter = ", ") => string.Join(delimiter, values.Where(v => v != null));
    }
}