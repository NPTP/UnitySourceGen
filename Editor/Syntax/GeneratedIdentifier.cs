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
    /// Type names go through <see cref="SanitizeTypeName"/> instead, which sanitizes each identifier in
    /// the name while leaving the punctuation that makes up generics, arrays and nullables intact.
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
        /// The reserved words that are also legitimate type names, so must never be escaped.
        /// </summary>
        private static readonly HashSet<string> predefinedTypeKeywords = new()
        {
            "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint", "nint", "nuint",
            "long", "ulong", "short", "ushort", "object", "string", "void"
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

        /// <summary>
        /// Sanitize a type name. Unlike a plain identifier a type name is structured - it can contain
        /// generic arguments, array ranks, nullable marks and namespace qualifiers - so each identifier
        /// within it is sanitized and everything holding them together is left alone.
        /// <code>
        /// SanitizeTypeName("Keyboard&amp;Mouse Actions")  // KeyboardMouseActions
        /// SanitizeTypeName("List&lt;My-Type&gt;")            // List&lt;MyType&gt;
        /// SanitizeTypeName("int?")                     // int?      (a type keyword is not escaped)
        /// SanitizeTypeName("BindingInfo[]")            // BindingInfo[]
        /// </code>
        /// </summary>
        public static string SanitizeTypeName(string rawTypeName)
        {
            if (string.IsNullOrEmpty(rawTypeName))
            {
                return FALLBACK;
            }

            StringBuilder result = new();
            StringBuilder identifier = new();

            foreach (char c in rawTypeName)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    identifier.Append(c);
                }
                else if (IsTypeNameStructure(c))
                {
                    AppendSanitizedIdentifier(result, identifier);

                    // A space is only meaningful after a comma, separating generic arguments. Anywhere
                    // else it would split one name into two, so it is dropped.
                    if (c != ' ' || (result.Length > 0 && result[result.Length - 1] == ','))
                    {
                        result.Append(c);
                    }
                }

                // Anything else - punctuation that means nothing in a type name - is dropped.
            }

            AppendSanitizedIdentifier(result, identifier);
            return result.Length == 0 ? FALLBACK : result.ToString();
        }

        /// <summary>The characters that give a type name its shape, rather than naming anything.</summary>
        private static bool IsTypeNameStructure(char c) => c is '<' or '>' or '[' or ']' or ',' or '.' or '?' or ' ';

        private static void AppendSanitizedIdentifier(StringBuilder result, StringBuilder identifier)
        {
            if (identifier.Length == 0)
            {
                return;
            }

            while (identifier.Length > 0 && char.IsDigit(identifier[0])) identifier.Remove(0, 1);

            if (identifier.Length > 0)
            {
                string segment = identifier.ToString();

                // Predefined type keywords - int, string, void - are legitimate type names and must not
                // be escaped. Any other reserved word used as a type name does need escaping.
                bool needsEscape = reservedKeywords.Contains(segment) && !predefinedTypeKeywords.Contains(segment);
                result.Append(needsEscape ? ESCAPE + segment : segment);
            }

            identifier.Clear();
        }

        /// <summary>Sanitize, then make the first character uppercase, for a type, property or method name.</summary>
        public static string SanitizeAsPascalCase(string rawName)
        {
            string identifier = Sanitize(rawName);
            return identifier[0] == ESCAPE || char.IsUpper(identifier[0])
                ? identifier
                : char.ToUpperInvariant(identifier[0]) + identifier.Substring(1);
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
