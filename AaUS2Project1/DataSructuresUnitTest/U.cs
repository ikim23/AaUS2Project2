using System;
using System.Collections.Generic;
using System.Linq;

namespace DataSructuresUnitTest
{
    static class U
    {
        public static string Format(this IEnumerable<int> e) => $"[{string.Join(",", e.Select(i => i.ToString().PadLeft(3)))}]";

        public static List<int> UniqueList(int count, int seed = 1)
        {
            var keyList = new List<int>(count);
            for (var i = 1; i <= count; i++)
                keyList.Add(i);
            var rand = new Random(seed);
            keyList.OrderBy(k => Guid.NewGuid());
            return keyList;
        }

        public static T Pop<T>(this List<T> list)
        {
            var value = list[0];
            list.RemoveAt(0);
            return value;
        }

        public static bool NextBool(this Random rand) => rand.Next() < 0.5;

    }
}
