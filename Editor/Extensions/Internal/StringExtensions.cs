using System;

namespace NPTP.UnitySourceGen.Editor.Extensions.Internal
{
    internal static class StringExtensions
    {
        internal static bool ContainsAll(this string s, params string[] others)
        {
            if (string.IsNullOrEmpty(s) || others.Length == 0)
                return false;

            foreach (string other in others)
            {
                if (!s.Contains(other))
                    return false;
            }

            return true;
        }

        internal static int GetIndentLevel(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;

            int whitespaceLength = s.Length - s.TrimStart().Length;
            if (whitespaceLength == 0)
                return 0;

            string whitespace = s.Substring(0, whitespaceLength);
            int tabs = 0;
            int spaces = 0;
            for (int i = 0; i < whitespace.Length; i++)
            {
                switch (whitespace[i])
                {
                    case ' ':
                        spaces++;
                        break;
                    case '\t':
                        tabs++;
                        break;
                }
            }

            return tabs + (int)Math.Ceiling((decimal)(spaces / 4));
        }
    }
}
