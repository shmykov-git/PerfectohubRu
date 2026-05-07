using Shared.Model.Enums;
using Shared.Model.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Calls.Extensions
{
    public static class ClientOptionsExtensions 
    { 
        public static (int, int) GetInterval(string interval)
        {
            string[] parts = interval.Trim('[', ']').Split(',');
            int from = int.Parse(parts[0]);
            int to = int.Parse(parts[1]);

            return (from, to);
        }

        public static string[] GetPhones(string phones)
        {
            return phones.Replace("*", "").Split(',').Select(v => v.Trim()).Where(v => !string.IsNullOrEmpty(v)).ToArray();
        }

        public static Dictionary<MarkType, string> GetMarks(string marksStr)
        {
            var parts = marksStr.Split(',');
            var marks = parts.Select(p => p.Trim().Split('-')).ToDictionary(vs => Enum.TryParse<MarkType>(vs[1], out var v) ? v : default, vs => vs[0]);
            var newDefaultMarks = Enum.GetValues(typeof(MarkType)).Cast<MarkType>().Where(v => !marks.Keys.Contains(v)).ToDictionary(v => v, v => v.GetDescription());

            foreach (var v in newDefaultMarks)
                marks[v.Key] = v.Value;

            return marks;
        }

        public static bool IsReplied(this ClientOptions o, int recallTryCount) => !o.IsNotReplied(recallTryCount);
        public static bool IsNotReplied(this ClientOptions o, int recallTryCount) => recallTryCount < o.RecallClientLimit;
    }
}
