using System;
using System.Collections.Generic;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Extensions;
using NPTP.UnitySourceGen.Editor.Extensions.Internal;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public abstract class GeneratableBase
    {
        protected const string SPACE = " ";
        protected const string SEMICOLON = ";";
        protected const string STATIC = "static";
        
        private const string OPEN_BRACE = "{";
        private const string CLOSE_BRACE = "}";
        private const int TAB_SPACES_COUNT = 4;

        internal string Name { get; }
        
        protected AccessModifier AccessModifier { get; }
        public bool IsStatic { get; }

        protected GeneratableBase(string name, AccessModifier accessModifier, bool isStatic)
        {
            Name = name;
            AccessModifier = accessModifier;
            IsStatic = isStatic;
        }

        public override string ToString() => GenerateStringRepresentation();
        public abstract string GenerateStringRepresentation();
        
        // TODO: Make this abstract, and clean it up
        public IEnumerable<string> GenerateStringRepresentationLines()
        {
            string[] lines = GenerateStringRepresentation().Split(Environment.NewLine);
            string[] linesWithoutLastLine = new string[lines.Length - 1];
            Array.Copy(lines, linesWithoutLastLine, linesWithoutLastLine.Length);
            return linesWithoutLastLine;
        }

        private string Tab(int count)
        {
            StringBuilder tab = new();
            for (int i = 0; i < TAB_SPACES_COUNT * count; i++) tab.Append(SPACE);
            return tab.ToString();
        }
        
        protected static string GetValueAsString<TValue>(Type type, TValue value)
        {
            StringBuilder sb = new();
            string left = string.Empty;
            string right = string.Empty;

            if (type == typeof(string))
            {
                left = right = "\"";
            }
            else if (type == typeof(float))
            {
                right = "f";
            }

            sb.Append(left);
            sb.Append(value);
            sb.Append(right);
            
            return sb.ToString();
        }
        
        protected void AddLine(StringBuilder sb, int indent, string line) => sb.AppendLine(Tab(indent) + line);
        
        protected void AddLines(StringBuilder sb, int indent, IEnumerable<string> lines) => lines.ForEach(line => AddLine(sb, indent, line));

        protected void AddEmptyLine(StringBuilder sb) => sb.AppendLine();

        protected void AddOpenBrace(StringBuilder sb, int indent) => AddLine(sb, indent, OPEN_BRACE);

        protected void AddCloseBrace(StringBuilder sb, int indent) => AddLine(sb, indent, CLOSE_BRACE);
        
        /// <summary>
        /// Kept for convenience. <see cref="TypeRef"/> is the general form, and is the only way to name a
        /// type that does not exist yet, such as another type being generated in the same run.
        /// </summary>
        protected static string GetTypeName(Type type) => TypeRef.From(type).Name;
    }
}