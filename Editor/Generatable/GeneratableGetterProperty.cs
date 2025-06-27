using System;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Options;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableGetterProperty<T> : GeneratableProperty
    {
        private const string GETTER_ARROW = "=>";
        
        private static Type PropertyType => typeof(T);

        private readonly string getterSyntax;
        
        internal GeneratableGetterProperty(NameSyntax nameSyntax, NameSyntax getterSyntax, AccessModifier getModifier, bool isStatic) : base(nameSyntax, getModifier, isStatic)
        {
            this.getterSyntax = getterSyntax.GetName();
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder property = new();

            property.Append(AccessModifier.AsString());
            if (IsStatic) property.Append(SPACE + STATIC);
            property.Append(SPACE + GetTypeName(PropertyType));
            property.Append(SPACE + Name);
            property.Append(SPACE + GETTER_ARROW);
            property.Append(SPACE + getterSyntax);
            property.Append(SEMICOLON);

            return property.ToString();
        }
    }
}