using System;
using System.Collections.Generic;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A generated enum, configured fluently on itself:
    /// <code>
    /// SourceGen.NewEnum("ControlScheme", AccessModifier.Public)
    ///     .InNamespace("MyGame.Enums")
    ///     .WithMember("None", -1)
    ///     .WithMember("KeyboardMouse", 0)
    /// </code>
    /// </summary>
    public sealed class GeneratableEnum : GeneratableDefinition
    {
        private class EnumMember
        {
            internal enum EnumValueMode
            {
                NonExplicit,
                ExplicitInt,
                ExplicitBitShiftFlag
            }

            private readonly string name;
            private readonly EnumValueMode valueMode;
            private readonly int value;
            private readonly int bitShiftValue;

            internal EnumMember(string name, EnumValueMode valueMode, int? value, int? bitShiftValue)
            {
                this.name = name;
                this.valueMode = valueMode;
                if (value.HasValue) this.value = value.Value;
                if (bitShiftValue.HasValue) this.bitShiftValue = bitShiftValue.Value;
            }

            public override string ToString()
            {
                return valueMode switch
                {
                    EnumValueMode.NonExplicit => name,
                    EnumValueMode.ExplicitInt => $"{name} = {value}",
                    EnumValueMode.ExplicitBitShiftFlag => $"{name} = {value} << {bitShiftValue}",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private const string ENUM = "enum";
        private const string FLAGS = "Flags";
        private const string SYSTEM = "System";

        private List<EnumMember> Members { get; } = new();

        private bool isFlags;

        internal GeneratableEnum(string name, AccessModifier accessModifier) : base(name, accessModifier, isStatic: false) { }

        #region File Placement

        public new GeneratableEnum InNamespace(string @namespace)
        {
            Namespace = @namespace;
            return this;
        }

        /// <summary>Write like WithDirective("UnityEngine"), rather than WithDirective("using UnityEngine;").</summary>
        public new GeneratableEnum WithDirective(string directive)
        {
            Directives.Add(directive);
            return this;
        }

        public new GeneratableEnum WithDirectives(params string[] directives)
        {
            if (directives != null)
            {
                foreach (string directive in directives) Directives.Add(directive);
            }

            return this;
        }

        #endregion

        #region Declaration

        public GeneratableEnum WithAccess(AccessModifier modifier)
        {
            AccessModifier = modifier;
            return this;
        }

        public GeneratableEnum Public() => WithAccess(AccessModifier.Public);
        public GeneratableEnum Private() => WithAccess(AccessModifier.Private);
        public GeneratableEnum Internal() => WithAccess(AccessModifier.Internal);

        /// <summary>Marks the enum [Flags], adding the System directive it needs.</summary>
        public GeneratableEnum AsFlags()
        {
            isFlags = true;
            Directives.Add(SYSTEM);
            return this;
        }

        public GeneratableEnum WithAttribute(AddableAttribute attribute)
        {
            AddAttribute(attribute);
            return this;
        }

        public GeneratableEnum WithAttribute(string attributeName, params string[] arguments) =>
            WithAttribute(new AddableAttribute(attributeName, arguments));

        public GeneratableEnum OnlyIf(string conditionalCompilationSymbol)
        {
            ConditionalCompilationSymbol = conditionalCompilationSymbol;
            return this;
        }

        #endregion

        #region Members

        /// <summary>A member with no explicit value, taking whatever the compiler assigns it.</summary>
        public GeneratableEnum WithMember(string memberName)
        {
            Members.Add(new EnumMember(memberName, EnumMember.EnumValueMode.NonExplicit, null, null));
            return this;
        }

        /// <summary>
        /// A member with an explicit value. Prefer this whenever generated code casts between the enum and
        /// an int, so the mapping cannot drift when members are added or reordered.
        /// </summary>
        public GeneratableEnum WithMember(string memberName, int value)
        {
            Members.Add(new EnumMember(memberName, EnumMember.EnumValueMode.ExplicitInt, value, null));
            return this;
        }

        /// <summary>e.g. WithBitShiftedMember("Gamepad", 1, 2) -> Gamepad = 1 &lt;&lt; 2</summary>
        public GeneratableEnum WithBitShiftedMember(string memberName, int value, int bitShiftValue)
        {
            Members.Add(new EnumMember(memberName, EnumMember.EnumValueMode.ExplicitBitShiftFlag, value, bitShiftValue));
            return this;
        }

        #endregion

        internal override void AppendTypeDeclaration(StringBuilder sb, int indent)
        {
            AppendIfDirective(sb);
            AddAttributeLines(sb, indent);
            AddEnumSignature(sb, indent);
            AddOpenBrace(sb, indent);

            indent++;
            AddEnumMembers(sb, indent);
            indent--;

            AddCloseBrace(sb, indent);

            if (HasConditionalCompilation)
            {
                AppendEndIfDirective(sb);
                sb.AppendLine();
            }
        }

        private void AddEnumSignature(StringBuilder sb, int indent)
        {
            if (isFlags) AddLine(sb, indent, $"[{FLAGS}]");
            AddLine(sb, indent, $"{AccessModifier.AsString()} {ENUM} {Name}");
        }

        private void AddEnumMembers(StringBuilder sb, int indent)
        {
            for (int i = 0; i < Members.Count; i++)
            {
                AddLine(sb, indent, Members[i] + (i < Members.Count - 1 ? COMMA : string.Empty));
            }
        }
    }
}
