using System;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.Extensions.Internal
{
    internal static class StringExtensions
    {
        internal static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            
            char firstChar = char.ToUpper(s[0]);
            if (s.Length == 1) return firstChar.ToString();
            return  firstChar + s[1..];
        }
        
        internal static string LowercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            
            char firstChar = char.ToLower(s[0]);
            if (s.Length == 1) return firstChar.ToString();
            return  firstChar + s[1..];
        }

        internal static string StringValueOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? string.Empty : s;
        }
        
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

        internal static bool CheckValidGenerationName(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                Debug.LogWarning("Tried to add a null- or empty-named element to a generatable type definition.");
                return false;
            }
            
            return true;
        }
    }
}