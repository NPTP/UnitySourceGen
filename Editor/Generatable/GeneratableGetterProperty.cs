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

        internal GeneratableGetterProperty(string name, AccessModifier getModifier, bool isStatic, GeneratableField accessedField) : base(name, getModifier, isStatic)
        {
            this.accessedField = accessedField;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder property = new();

            property.Append(AccessModifier.AsString());
            if (IsStatic) property.Append(SPACE + STATIC);
            property.Append(SPACE + GetTypeName(PropertyType));
            property.Append(SPACE + Name);
            property.Append(SPACE + GETTER_ARROW);
            property.Append(SPACE + accessedField.Name);
            property.Append(SEMICOLON);
                                    
            return property.ToString();
        }
    }
}