using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Options;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public abstract class GeneratableField : GeneratableBase
    {
        protected GeneratableField(NameSyntax nameSyntax, AccessModifier accessModifier, bool isStatic) : base(nameSyntax, accessModifier, isStatic) { }
    }
    
    public class GeneratableField<T> : GeneratableField
    {
        private readonly bool hasInitialValue;
        private readonly T initialValue;
        private List<AddableAttribute> attributes;

        private bool HasAttributes => attributes is { Count: > 0 };

        private static Type FieldType => typeof(T);
        
        internal GeneratableField(NameSyntax nameSyntax, AccessModifier accessModifier, bool isStatic) : base(nameSyntax, accessModifier, isStatic)
        {
            hasInitialValue = false;
        }

        internal GeneratableField(NameSyntax nameSyntax, AccessModifier accessModifier, bool isStatic, T initialValue) : base(nameSyntax, accessModifier, isStatic)
        {
            this.initialValue = initialValue;
            hasInitialValue = true;
        }

        public void AddAttribute(AddableAttribute addableAttribute)
        {
            if (attributes != null && attributes.Any(a => Equals(a, addableAttribute)))
            {
                return;
            }
            
            attributes ??= new();
            attributes.Add(addableAttribute);
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder field = new();

            if (HasAttributes)
            {
                foreach (var attribute in attributes) field.Append(attribute.GetStringRepresentation() + SPACE);
            }
            
            field.Append(AccessModifier.AsString());
            PrependAdditionalLabels(field);
            if (IsStatic) field.Append(SPACE + STATIC);
            field.Append(SPACE + GetTypeName(FieldType));
            field.Append(SPACE + Name);

            if (hasInitialValue)
            {
                field.Append(SPACE + "=" + SPACE);
                field.Append(GetValueAsString(typeof(T), initialValue));
            }
            
            field.Append(SEMICOLON);

            return field.ToString();
        }

        protected virtual void PrependAdditionalLabels(StringBuilder fieldStringBuilder) { }
    }
}