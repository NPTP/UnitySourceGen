using System.Collections.Generic;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A top-level type that can stand alone in a file: it owns the using directives and namespace written
    /// around it. Concrete types re-declare the placement methods so that chaining keeps their own type.
    /// </summary>
    public abstract class GeneratableDefinition : GeneratableBase
    {
        internal SortedSet<string> Directives { get; } = new();
        internal string Namespace { get; set; }

        /// <summary>A top-level type with no access modifier set is internal, as in C#.</summary>
        internal GeneratableDefinition(string name) : base(name)
        {
            AccessModifier = AccessModifier.Internal;
        }

        public GeneratableDefinition InNamespace(string @namespace)
        {
            Namespace = @namespace;
            return this;
        }

        /// <summary>Write like WithDirective("UnityEngine"), rather than WithDirective("using UnityEngine;").</summary>
        public GeneratableDefinition WithDirective(string directive)
        {
            Directives.Add(directive);
            return this;
        }

        public GeneratableDefinition WithDirectives(params string[] directives)
        {
            if (directives != null)
            {
                foreach (string directive in directives) Directives.Add(directive);
            }

            return this;
        }

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

        /// <summary>
        /// Every directive this definition needs: the ones added by hand, plus the namespaces of any types
        /// it names through a real System.Type rather than a raw string.
        /// </summary>
        internal SortedSet<string> GetAllDirectives()
        {
            SortedSet<string> allDirectives = new(Directives);
            if (this is GeneratableTypeDefinition typeDefinition)
            {
                foreach (string requiredNamespace in typeDefinition.GetRequiredNamespaces()) allDirectives.Add(requiredNamespace);
            }

            return allDirectives;
        }

        protected void AddUsingDirectives(StringBuilder sb, int indent)
        {
            SortedSet<string> allDirectives = GetAllDirectives();
            foreach (string directive in allDirectives)
                AddLine(sb, indent, $"using {directive};");

            if (allDirectives.Count > 0)
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
