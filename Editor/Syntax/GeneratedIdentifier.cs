using System.Collections.Generic;
using System.Text;

namespace NPTP.UnitySourceGen.Editor.Syntax
{
    /// <summary>
    /// Turns a name from outside the code - an asset name, a field in a config file, anything the user
    /// typed - into an identifier that is guaranteed to compile. Every generatable runs its name through
    /// this, so a generator never has to sanitize by hand.
    /// <code>
    /// GeneratedIdentifier.Sanitize("Keyboard&amp;Mouse")  // KeyboardMouse
    /// GeneratedIdentifier.Sanitize("2D Movement")     // DMovement
    /// GeneratedIdentifier.Sanitize("class")           // @class
    /// GeneratedIdentifier.Sanitize("!!!")             // Unnamed
    /// </code>
    /// Type names are deliberately not sanitized: they can legitimately contain generics, arrays and
    /// nullables, so <see cref="TypeRef"/> takes them as written.
    /// </summary>
    public static class GeneratedIdentifier
    {
        private const string FALLBACK = "Unnamed";
        private const char ESCAPE = '@';

        /// <summary>
        /// Contextual keywords are legal as identifiers, so only reserved words need escaping.
        /// </summary>
        private static readonly HashSet<string> reservedKeywords = new()
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
            "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
            "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
            "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new",
            "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static",
            "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
            "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
        };

        /// <summary>
        /// Strip anything that cannot appear in an identifier, drop leading digits, and escape reserved
        /// words. Underscores are kept: unlike punctuation they are valid in identifiers.
        /// </summary>
        public static string Sanitize(string rawName)
        {
            if (string.IsNullOrEmpty(rawName))
            {
                return FALLBACK;
            }

            StringBuilder sb = new();
            foreach (char c in rawName)
            {
                if (char.IsLetterOrDigit(c) || c == '_') sb.Append(c);
            }

            // A digit is never valid as the first character, and there can be more than one.
            while (sb.Length > 0 && char.IsDigit(sb[0])) sb.Remove(0, 1);

            if (sb.Length == 0)
            {
                return FALLBACK;
            }

            string identifier = sb.ToString();
            return reservedKeywords.Contains(identifier) ? ESCAPE + identifier : identifier;
        }

        /// <summary>Sanitize, then make the first character lowercase, for a field or parameter name.</summary>
        public static string SanitizeAsCamelCase(string rawName)
        {
            string identifier = Sanitize(rawName);
            return identifier[0] == ESCAPE || char.IsLower(identifier[0])
                ? identifier
                : char.ToLowerInvariant(identifier[0]) + identifier.Substring(1);
        }
    }
}
