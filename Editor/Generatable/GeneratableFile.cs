using System.Collections.Generic;
using System.Text;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// One source file containing any number of types, which may sit in different namespaces. Writing a
    /// single <see cref="GeneratableDefinition"/> straight to a file works when it is alone; use this when
    /// a file needs, say, an enum plus its extensions class, or extension classes for two namespaces.
    /// <code>
    /// GeneratableFile file = SourceGen.NewFile()
    ///     .WithHeaderComment("Auto-generated. Do not edit.")
    ///     .Containing(controlSchemeEnum, controlSchemeExtensions);
    /// SourceGen.WriteToPath("Assets/MyGame.Generated/ControlScheme.cs", file);
    /// </code>
    /// Using directives are collected from every contained type and hoisted to the top of the file, since
    /// C# does not allow them after the first namespace declaration in the common style.
    /// </summary>
    public class GeneratableFile
    {
        private readonly List<GeneratableDefinition> definitions = new();
        private readonly SortedSet<string> directives = new();
        private readonly List<string> headerComments = new();

        private string conditionalCompilationSymbol;

        internal GeneratableFile() { }

        public GeneratableFile Containing(params GeneratableDefinition[] generatableDefinitions)
        {
            if (generatableDefinitions == null)
            {
                return this;
            }

            foreach (GeneratableDefinition definition in generatableDefinitions)
            {
                if (definition != null) definitions.Add(definition);
            }

            return this;
        }

        /// <summary>Write like WithDirective("UnityEngine"), rather than WithDirective("using UnityEngine;").</summary>
        public GeneratableFile WithDirective(string directive)
        {
            directives.Add(directive);
            return this;
        }

        public GeneratableFile WithDirectives(params string[] fileDirectives)
        {
            if (fileDirectives == null)
            {
                return this;
            }

            foreach (string directive in fileDirectives) directives.Add(directive);
            return this;
        }

        /// <summary>A comment line placed above the first namespace, e.g. a generator notice.</summary>
        public GeneratableFile WithHeaderComment(params string[] commentLines)
        {
            if (commentLines == null)
            {
                return this;
            }

            foreach (string line in commentLines) headerComments.Add(line);
            return this;
        }

        /// <summary>
        /// Wrap the entire file, using directives included, in "#if SYMBOL" / "#endif". This is how a file
        /// of editor-only code lives in a runtime assembly without reaching a build.
        /// </summary>
        public GeneratableFile OnlyIf(string symbol)
        {
            conditionalCompilationSymbol = symbol;
            return this;
        }

        public string GenerateStringRepresentation()
        {
            StringBuilder sb = new();

            bool isConditional = !string.IsNullOrEmpty(conditionalCompilationSymbol);
            if (isConditional) sb.AppendLine($"#if {conditionalCompilationSymbol}");

            AppendDirectives(sb);
            AppendHeaderComments(sb);
            AppendNamespaceGroups(sb);

            if (isConditional) sb.AppendLine("#endif");

            return sb.ToString();
        }

        public override string ToString() => GenerateStringRepresentation();

        private void AppendDirectives(StringBuilder sb)
        {
            SortedSet<string> allDirectives = new(directives);
            foreach (GeneratableDefinition definition in definitions)
            {
                foreach (string directive in definition.Directives) allDirectives.Add(directive);
            }

            if (allDirectives.Count == 0)
            {
                return;
            }

            foreach (string directive in allDirectives) sb.AppendLine($"using {directive};");
            sb.AppendLine();
        }

        private void AppendHeaderComments(StringBuilder sb)
        {
            foreach (string line in headerComments) sb.AppendLine(line);
        }

        /// <summary>
        /// Consecutive types sharing a namespace go into one namespace block, in the order they were added.
        /// A change of namespace closes the current block and opens the next.
        /// </summary>
        private void AppendNamespaceGroups(StringBuilder sb)
        {
            string currentNamespace = null;
            bool inNamespaceBlock = false;
            bool isFirstTypeInBlock = true;

            foreach (GeneratableDefinition definition in definitions)
            {
                string definitionNamespace = definition.Namespace;

                if (!inNamespaceBlock || definitionNamespace != currentNamespace)
                {
                    if (inNamespaceBlock)
                    {
                        sb.AppendLine("}");
                        inNamespaceBlock = false;
                    }

                    if (currentNamespace != null) sb.AppendLine();

                    currentNamespace = definitionNamespace;
                    isFirstTypeInBlock = true;

                    if (!string.IsNullOrEmpty(currentNamespace))
                    {
                        sb.AppendLine($"namespace {currentNamespace}");
                        sb.AppendLine("{");
                        inNamespaceBlock = true;
                    }
                }

                if (!isFirstTypeInBlock) sb.AppendLine();
                isFirstTypeInBlock = false;

                definition.AppendTypeDeclaration(sb, inNamespaceBlock ? 1 : 0);
            }

            if (inNamespaceBlock) sb.AppendLine("}");
        }
    }
}
