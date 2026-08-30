using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A generated field, configured fluently on itself:
    /// <code>
    /// SourceGen.NewField("playerID", "int").Private().WithInitialValue("0")
    /// SourceGen.NewField("actions", "GameplayActions").Serialized()
    /// </code>
    /// </summary>
    public class GeneratableField : GeneratableBase
    {
        private const string CONST = "const";
        private const string READONLY = "readonly";
        private const string SERIALIZE_FIELD = "SerializeField";

        private readonly TypeRef fieldType;

        private string initialValueExpression;
        private bool isConst;
        private bool isReadOnly;

        private bool HasInitialValue => initialValueExpression != null;

        internal GeneratableField(string name, TypeRef fieldType) : base(name, AccessModifier.Private, isStatic: false)
        {
            this.fieldType = fieldType;
        }

        public GeneratableField WithAccess(AccessModifier modifier)
        {
            AccessModifier = modifier;
            return this;
        }

        public GeneratableField Public() => WithAccess(AccessModifier.Public);
        public GeneratableField Private() => WithAccess(AccessModifier.Private);
        public GeneratableField Protected() => WithAccess(AccessModifier.Protected);
        public GeneratableField Internal() => WithAccess(AccessModifier.Internal);

        public GeneratableField Static()
        {
            IsStatic = true;
            return this;
        }

        public GeneratableField ReadOnly()
        {
            isReadOnly = true;
            return this;
        }

        /// <summary>A const field. Implies an initial value, which C# requires.</summary>
        public GeneratableField Const(string valueExpression)
        {
            isConst = true;
            initialValueExpression = valueExpression;
            return this;
        }

        /// <summary>
        /// Written verbatim on the right of the assignment, so it must already be valid C#, e.g.
        /// "new()", "\"literal\"" or "3f".
        /// </summary>
        public GeneratableField WithInitialValue(string valueExpression)
        {
            initialValueExpression = valueExpression;
            return this;
        }

        /// <summary>The Unity-specific shorthand for a field with [SerializeField] on it.</summary>
        public GeneratableField Serialized() => WithAttribute(SERIALIZE_FIELD);

        public GeneratableField WithAttribute(AddableAttribute attribute)
        {
            AddAttribute(attribute);
            return this;
        }

        public GeneratableField WithAttribute(string attributeName, params string[] arguments) =>
            WithAttribute(new AddableAttribute(attributeName, arguments));

        public GeneratableField OnlyIf(string conditionalCompilationSymbol)
        {
            ConditionalCompilationSymbol = conditionalCompilationSymbol;
            return this;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder field = new();

            field.Append(GetAttributesInline());
            field.Append(AccessModifier.AsString());
            if (isConst) field.Append(SPACE + CONST);
            if (IsStatic) field.Append(SPACE + STATIC);
            if (isReadOnly) field.Append(SPACE + READONLY);
            field.Append(SPACE + fieldType.Name);
            field.Append(SPACE + Name);

            if (HasInitialValue)
            {
                field.Append(SPACE + "=" + SPACE);
                field.Append(initialValueExpression);
            }

            field.Append(SEMICOLON);

            return field.ToString();
        }
    }
}
