using System;
using System.Collections.Generic;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using System.Linq;
using NPTP.UnitySourceGen.Editor.Extensions.Internal;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// Anything that can be written into a generated file: a type, or a member of one. Everything shared
    /// across all of them lives here - the name, access modifier, static-ness, attributes, conditional
    /// compilation, and the indentation helpers used to render them.
    /// <para>
    /// Generatables are created through <see cref="SourceGen"/>, which takes only a name, and then
    /// configure themselves through fluent methods - there are no separate builder types and no extension
    /// classes to import. Anything not set takes the same default C# would apply: members are private,
    /// top-level types internal, methods return void.
    /// </para>
    /// <para>
    /// The name is run through <see cref="GeneratedIdentifier"/> on construction, so a name taken from an
    /// asset or a config file cannot produce code that fails to compile. Type names are the exception and
    /// are never sanitized - see <see cref="TypeRef"/>.
    /// </para>
    /// </summary>
    public abstract class GeneratableBase
    {
        protected const string SPACE = " ";
        protected const string SEMICOLON = ";";
        protected const string COMMA = ",";
        protected const string STATIC = "static";

        private const string OPEN_BRACE = "{";
        private const string CLOSE_BRACE = "}";
        private const int TAB_SPACES_COUNT = 4;

        internal string Name { get; }

        /// <summary>
        /// Identifies the member for duplicate detection. Name alone is right for fields, properties and
        /// events, which cannot be overloaded; methods override this to include their signature so that
        /// genuine overloads are not silently dropped.
        /// </summary>
        internal virtual string DedupeKey => Name;

        /// <summary>
        /// The types this member names in its own declaration. Used to work out which using directives the
        /// containing type or file needs, so callers do not have to list them by hand.
        /// </summary>
        internal virtual IEnumerable<TypeRef> ReferencedTypes => Enumerable.Empty<TypeRef>();

        private List<AddableAttribute> attributes;

        protected bool HasAttributes => attributes is { Count: > 0 };

        /// <summary>
        /// When set, this member is wrapped in "#if SYMBOL" / "#endif". Preprocessor directives are always
        /// written at column 0, which is how C# conventionally formats them regardless of nesting.
        /// </summary>
        internal string ConditionalCompilationSymbol { get; set; }

        protected bool HasConditionalCompilation => !string.IsNullOrEmpty(ConditionalCompilationSymbol);

        protected void AppendIfDirective(StringBuilder sb)
        {
            if (HasConditionalCompilation) sb.AppendLine($"#if {ConditionalCompilationSymbol}");
        }

        protected void AppendEndIfDirective(StringBuilder sb)
        {
            if (HasConditionalCompilation) sb.Append("#endif");
        }

        // Settable so that each generatable can configure itself fluently after construction.
        protected AccessModifier AccessModifier { get; set; } = AccessModifier.Private;
        public bool IsStatic { get; protected set; }

        /// <summary>
        /// The name is the only thing a generatable needs up front, and it is sanitized into a valid
        /// identifier here so that no caller has to. Everything else is set fluently afterwards.
        /// </summary>
        protected GeneratableBase(string name) : this(name, sanitizeName: true) { }

        /// <param name="sanitizeName">
        /// False only for generatables whose "name" is not an identifier at all, such as a comment.
        /// </param>
        protected GeneratableBase(string name, bool sanitizeName)
        {
            Name = sanitizeName ? GeneratedIdentifier.Sanitize(name) : name;
        }

        /// <summary>
        /// Attach an attribute. Where it is written depends on the member: inline before a field, on its
        /// own line above a method, type or event.
        /// </summary>
        public void AddAttribute(AddableAttribute addableAttribute)
        {
            if (addableAttribute == null || (attributes != null && attributes.Any(existing => Equals(existing, addableAttribute))))
            {
                return;
            }

            attributes ??= new List<AddableAttribute>();
            attributes.Add(addableAttribute);
        }

        /// <summary>Attributes written before the member on the same line, e.g. "[SerializeField] ".</summary>
        protected string GetAttributesInline()
        {
            if (!HasAttributes)
            {
                return string.Empty;
            }

            StringBuilder sb = new();
            foreach (AddableAttribute attribute in attributes) sb.Append(attribute.GetStringRepresentation() + SPACE);
            return sb.ToString();
        }

        /// <summary>Attributes written one per line above the member.</summary>
        protected void AddAttributeLines(StringBuilder sb, int indent)
        {
            if (!HasAttributes)
            {
                return;
            }

            foreach (AddableAttribute attribute in attributes) AddLine(sb, indent, attribute.GetStringRepresentation());
        }

        public override string ToString() => GenerateStringRepresentation();
        public abstract string GenerateStringRepresentation();

        /// <summary>
        /// The generated representation split into lines. Members built with AppendLine finish with a
        /// trailing newline, which would otherwise show up as a spurious empty line; single-line members
        /// such as expression-bodied methods have no trailing newline, so only an actual empty final
        /// element is dropped.
        /// </summary>
        public IEnumerable<string> GenerateStringRepresentationLines()
        {
            List<string> lines = new(GenerateStringRepresentation().Split(Environment.NewLine));
            if (lines.Count > 0 && string.IsNullOrEmpty(lines[lines.Count - 1]))
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return lines;
        }

        private string Tab(int count)
        {
            StringBuilder tab = new();
            for (int i = 0; i < TAB_SPACES_COUNT * count; i++) tab.Append(SPACE);
            return tab.ToString();
        }

        protected void AddLine(StringBuilder sb, int indent, string line) => sb.AppendLine(Tab(indent) + line);

        protected void AddLines(StringBuilder sb, int indent, IEnumerable<string> lines) => lines.ForEach(line => AddLine(sb, indent, line));

        protected void AddEmptyLine(StringBuilder sb) => sb.AppendLine();

        protected void AddOpenBrace(StringBuilder sb, int indent) => AddLine(sb, indent, OPEN_BRACE);

        protected void AddCloseBrace(StringBuilder sb, int indent) => AddLine(sb, indent, CLOSE_BRACE);
    }
}