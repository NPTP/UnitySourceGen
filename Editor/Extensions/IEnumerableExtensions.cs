using System;
using System.Collections.Generic;
using System.Text;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> iEnumerable, Action<T> action)
        {
            foreach (T variable in iEnumerable)
            {
                action.Invoke(variable);
            }
        }

        public static string LinesToString(this IEnumerable<string> lines)
        {
            StringBuilder sb = new();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }
}
