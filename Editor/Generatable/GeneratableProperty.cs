using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A generated property, either auto-implemented or expression-bodied:
    /// <code>
    /// SourceGen.NewProperty("Gameplay", "GameplayActions").Public().GetOnly()
    /// SourceGen.NewProperty("Gameplay", "GameplayActions").Public().Static().Expression("DefaultPlayer.Gameplay()")
    /// </code>
    /// </summary>
    public class GeneratableProperty : GeneratableBase
    {
        private const string GETTER_ARROW = "=>";

        private readonly TypeRef propertyType;

        private string expression;
        private bool hasSetter;
        private bool isSetterPrivate;

        internal GeneratableProperty(string name, TypeRef propertyType) : base(name, AccessModifier.Private, isStatic: false)
        {
            this.propertyType = propertyType;
        }

        public GeneratableProperty WithAccess(AccessModifier modifier)
        {
            AccessModifier = modifier;
            return this;
        }

        public GeneratableProperty Public() => WithAccess(AccessModifier.Public);
        public GeneratableProperty Private() => WithAccess(AccessModifier.Private);
        public GeneratableProperty Protected() => WithAccess(AccessModifier.Protected);
        public GeneratableProperty Internal() => WithAccess(AccessModifier.Internal);

        public GeneratableProperty Static()
        {
            IsStatic = true;
            return this;
        }

        /// <summary>An auto-property with only a getter: "public int Foo { get; }".</summary>
        public GeneratableProperty GetOnly()
        {
            expression = null;
            hasSetter = false;
            return this;
        }

        /// <summary>An auto-property with both accessors: "public int Foo { get; set; }".</summary>
        public GeneratableProperty GetSet()
        {
            expression = null;
            hasSetter = true;
            isSetterPrivate = false;
            return this;
        }

        /// <summary>An auto-property with a private setter: "public int Foo { get; private set; }".</summary>
        public GeneratableProperty GetPrivateSet()
        {
            expression = null;
            hasSetter = true;
            isSetterPrivate = true;
            return this;
        }

        /// <summary>An expression-bodied property: "public int Foo => expression;".</summary>
        public GeneratableProperty Expression(string getterExpression)
        {
            expression = getterExpression;
            return this;
        }

        public GeneratableProperty WithAttribute(AddableAttribute attribute)
        {
            AddAttribute(attribute);
            return this;
        }

        public GeneratableProperty WithAttribute(string attributeName, params string[] arguments) =>
            WithAttribute(new AddableAttribute(attributeName, arguments));

        public GeneratableProperty OnlyIf(string conditionalCompilationSymbol)
        {
            ConditionalCompilationSymbol = conditionalCompilationSymbol;
            return this;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder property = new();

            property.Append(GetAttributesInline());
            property.Append(AccessModifier.AsString());
            if (IsStatic) property.Append(SPACE + STATIC);
            property.Append(SPACE + propertyType.Name);
            property.Append(SPACE + Name);

            if (expression != null)
            {
                property.Append(SPACE + GETTER_ARROW);
                property.Append(SPACE + expression);
                property.Append(SEMICOLON);
                return property.ToString();
            }

            property.Append(SPACE + "{ get;");
            if (hasSetter) property.Append(isSetterPrivate ? SPACE + "private set;" : SPACE + "set;");
            property.Append(SPACE + "}");

            return property.ToString();
        }
    }
}
