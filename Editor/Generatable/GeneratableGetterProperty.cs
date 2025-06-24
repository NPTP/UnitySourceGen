using System;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableGetterProperty<T> : GeneratableProperty
    {
        private const string GETTER_ARROW = "=>";
        
        private static Type PropertyType => typeof(T);
        
        private readonly GeneratableField accessedField;
        private readonly T getterValue;

        internal GeneratableGetterProperty(string name, AccessModifier getModifier, bool isStatic, GeneratableField accessedField) : base(name, getModifier, isStatic)
        {
            this.accessedField = accessedField;
        }

        internal GeneratableGetterProperty(string name, AccessModifier accessModifier, bool isStatic, T getterValue) : base(name, accessModifier, isStatic)
        {
            this.getterValue = getterValue;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder property = new();

            property.Append(AccessModifier.AsString());
            if (IsStatic) property.Append(SPACE + STATIC);
            property.Append(SPACE + GetTypeName(PropertyType));
            property.Append(SPACE + Name);
            property.Append(SPACE + GETTER_ARROW);
            
            if (accessedField != null) property.Append(SPACE + accessedField.Name);
            else if (getterValue != null) property.Append(SPACE + GetValueAsString(typeof(T), getterValue));
            
            property.Append(SEMICOLON);

            return property.ToString();
        }
    }
}