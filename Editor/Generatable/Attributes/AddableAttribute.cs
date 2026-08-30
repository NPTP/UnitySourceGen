using System;
using System.Linq;

namespace NPTP.UnitySourceGen.Editor.Generatable.Attributes
{
    /// <summary>
    /// An attribute to place on a generated member, with optional arguments.
    /// <code>
    /// new AddableAttribute("SerializeField")
    /// new AddableAttribute("RuntimeInitializeOnLoadMethod", "RuntimeInitializeLoadType.BeforeSceneLoad")
    /// new AddableAttribute("MenuItem", AddableAttribute.StringArgument("Input/Regenerate"),
    ///                                  AddableAttribute.NamedArgument("isValidateFunction", "false"), "100")
    /// </code>
    /// Arguments are written verbatim, so each must already be a valid C# expression. Use
    /// <see cref="StringArgument"/> to get a correctly quoted and escaped string literal.
    /// </summary>
    public class AddableAttribute
    {
        public string Name { get; }

        private readonly string[] arguments;

        public AddableAttribute(string attributeName, params string[] arguments)
        {
            Name = attributeName;
            this.arguments = arguments ?? Array.Empty<string>();
        }

        /// <summary>Quotes and escapes a value so it can be passed as a string literal argument.</summary>
        public static string StringArgument(string value)
        {
            string escaped = (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
            return $"\"{escaped}\"";
        }

        /// <summary>e.g. NamedArgument("isValidateFunction", "false") -> isValidateFunction: false</summary>
        public static string NamedArgument(string parameterName, string valueExpression) => $"{parameterName}: {valueExpression}";

        public string GetStringRepresentation()
        {
            return arguments.Length == 0
                ? $"[{Name}]"
                : $"[{Name}({string.Join(", ", arguments)})]";
        }

        public override string ToString() => GetStringRepresentation();

        protected bool Equals(AddableAttribute other)
        {
            return other != null && Name == other.Name && arguments.SequenceEqual(other.arguments);
        }

        public override bool Equals(object obj) => obj is AddableAttribute other && Equals(other);

        public override int GetHashCode() => GetStringRepresentation().GetHashCode();
    }
}
