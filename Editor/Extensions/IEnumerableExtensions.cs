using System;
using System.Collections.Generic;

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
    }
}
