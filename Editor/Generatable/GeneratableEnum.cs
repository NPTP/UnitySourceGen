using System;
using System.Collections.Generic;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public sealed class GeneratableEnum : GeneratableDefinition
    {
        internal class EnumMember
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
        private const string FLAGS = "[Flags]";
        private const string SYSTEM = "System";

        private List<EnumMember> Members { get; } = new();

        private bool isFlags;
        internal bool IsFlags
        {
            get => isFlags;
            set
            {
                isFlags = value;
                if (value) Directives.Add(SYSTEM);
            }
        }

        internal GeneratableEnum(string nameSyntax, AccessModifier accessModifier) : base(nameSyntax, accessModifier, isStatic: false) { }

        internal void AddMember(string name, EnumMember.EnumValueMode valueMode, int? value, int? bitShiftValue)
        {
            Members.Add(new EnumMember(name, valueMode, value, bitShiftValue));
        }

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

            AddEnumSignature(sb, indent);
            AddOpenBrace(sb, indent);
            
            indent++;
            AddEnumMembers(sb, indent);
            indent--;
            
            AddCloseBrace(sb, indent);
            
            if (HasNamespace())
            {
                indent--;
                AddCloseBrace(sb, indent);
            }
            
            return sb.ToString();
        }

        private void AddEnumSignature(StringBuilder sb, int indent)
        {
            if (IsFlags) AddLine(sb, indent, FLAGS);
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