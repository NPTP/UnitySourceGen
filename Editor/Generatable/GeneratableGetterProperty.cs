using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Syntax;
using System.Text;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableGetterProperty : GeneratableProperty
    {
        private const string GETTER_ARROW = "=>";

        private readonly TypeRef propertyType;
        private readonly string fieldName;
        private readonly CustomSyntax getterValueSyntax;

        internal GeneratableGetterProperty(string name, TypeRef propertyType, string fieldName, AccessModifier getModifier, bool isStatic, CustomSyntax getterValueSyntax)
            : base(name, getModifier, isStatic)
        {
            this.propertyType = propertyType;
            this.fieldName = fieldName;
            this.getterValueSyntax = getterValueSyntax;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder property = new();

            property.Append(AccessModifier.AsString());
            if (IsStatic) property.Append(SPACE + STATIC);
            property.Append(SPACE + propertyType.Name);
            property.Append(SPACE + Name);
            property.Append(SPACE + GETTER_ARROW);
            property.Append(SPACE + getterValueSyntax.InSyntax(fieldName));
            property.Append(SEMICOLON);

            return property.ToString();
        }
    }

    /// <summary>
    /// A getter property whose type is a real compiled type. Use the non-generic
    /// <see cref="GeneratableGetterProperty"/> for a type that is itself being generated.
    /// </summary>
    public class GeneratableGetterProperty<T> : GeneratableGetterProperty
    {
        internal GeneratableGetterProperty(string name, string fieldName, AccessModifier getModifier, bool isStatic, CustomSyntax getterValueSyntax)
            : base(name, TypeRef.From(typeof(T)), fieldName, getModifier, isStatic, getterValueSyntax) { }
    }
}
