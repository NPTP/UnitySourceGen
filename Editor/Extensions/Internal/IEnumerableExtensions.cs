using System;
using System.Collections.Generic;

namespace NPTP.UnitySourceGen.Editor.Extensions.Internal
{
    internal static class IEnumerableExtensions
    {
        internal static void ForEach<T>(this IEnumerable<T> iEnumerable, Action<T> action)
        {
            foreach (T variable in iEnumerable)
            {
                action.Invoke(variable);
            }
        }
    }
}
