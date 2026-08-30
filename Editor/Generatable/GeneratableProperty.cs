using System.Collections.Generic;
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
    /// SourceGen.NewProperty("Enabled", "bool").Public().WithAccessors("runtime.Enabled", "runtime.Enabled = value")
    /// </code>
    /// Auto-property forms are <see cref="GetOnly"/>, <see cref="GetSet"/> and
    /// <see cref="GetPrivateSet"/>; the others give the accessors an expression body.
    /// </summary>
    public class GeneratableProperty : GeneratableBase
    {
        private const string GETTER_ARROW = "=>";

        private readonly TypeRef propertyType;

        private string expression;
        private string getterExpression;
        private string setterExpression;
        private bool hasSetter;

        internal override IEnumerable<TypeRef> ReferencedTypes => new[] { propertyType };

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
        public GeneratableProperty Expression(string propertyExpression)
        {
            expression = propertyExpression;
            return this;
        }

        /// <summary>
        /// Accessors that each forward to an expression:
        /// "public int Foo { get => target; set => target = value; }". Pass a null setter for a
        /// get-only property that still needs an explicit accessor body.
        /// </summary>
        public GeneratableProperty WithAccessors(string getExpression, string setExpression)
        {
            getterExpression = getExpression;
            setterExpression = setExpression;
            expression = null;
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

            if (getterExpression != null)
            {
                property.Append(SPACE + "{ get => " + getterExpression + SEMICOLON);
                if (setterExpression != null) property.Append(SPACE + "set => " + setterExpression + SEMICOLON);
                property.Append(SPACE + "}");
                return property.ToString();
            }

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
