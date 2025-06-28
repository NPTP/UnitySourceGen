using System;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableGetterProperty<T> : GeneratableProperty
    {
        private const string GETTER_ARROW = "=>";
        
        private static Type PropertyType => typeof(T);

        private readonly string fieldName;
        private readonly CustomSyntax getterValueSyntax;

        internal GeneratableGetterProperty(string name, string fieldName, AccessModifier getModifier, bool isStatic, CustomSyntax getterValueSyntax) : base(name, getModifier, isStatic)
        {
            this.fieldName = fieldName;
            this.getterValueSyntax = getterValueSyntax;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder property = new();

            property.Append(AccessModifier.AsString());
            if (IsStatic) property.Append(SPACE + STATIC);
            property.Append(SPACE + GetTypeName(PropertyType));
            property.Append(SPACE + Name);
            property.Append(SPACE + GETTER_ARROW);
            property.Append(SPACE + getterValueSyntax.InSyntax(fieldName));
            property.Append(SEMICOLON);

            return property.ToString();
        }
    }
}