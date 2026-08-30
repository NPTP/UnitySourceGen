using System.Collections.Generic;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public abstract class GeneratableDefinition : GeneratableBase
    {
        // TODO: Adding directives, fields, types etc should auto-add directives
        internal SortedSet<string> Directives { get; } = new();
        internal string Namespace { get; set; }

        internal GeneratableDefinition(string name, AccessModifier accessModifier, bool isStatic) : base(name, accessModifier, isStatic) { }

        /// <summary>
        /// Append just this type - its signature, braces and members - at the given indent, without any
        /// using directives or namespace around it. This is what lets several types share one file.
        /// </summary>
        internal abstract void AppendTypeDeclaration(StringBuilder sb, int indent);

        public override string GenerateStringRepresentation()
        {
            int indent = 0;
            StringBuilder sb = new();

            AddUsingDirectives(sb, indent);
            AddNamespace(sb, indent);
            if (HasNamespace())
            {
                AddOpenBrace(sb, indent);
                indent++;
            }

            AppendTypeDeclaration(sb, indent);

            if (HasNamespace())
            {
                indent--;
                AddCloseBrace(sb, indent);
            }

            return sb.ToString();
        }

        protected void AddUsingDirectives(StringBuilder sb, int indent)
        {
            foreach (string directive in Directives)
                AddLine(sb, indent, $"using {directive};");

            if (Directives.Count > 0)
                AddEmptyLine(sb);
        }

        protected void AddNamespace(StringBuilder sb, int indent)
        {
            if (!HasNamespace()) return;
            AddLine(sb, indent, $"namespace {Namespace}");
        }

        protected bool HasNamespace() => !string.IsNullOrEmpty(Namespace);
    }
}