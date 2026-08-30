using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableField : GeneratableBase
    {
        private readonly TypeRef fieldType;
        private readonly string initialValueExpression;

        private bool HasInitialValue => initialValueExpression != null;

        /// <param name="initialValueExpression">
        /// Written verbatim on the right of the assignment, so it must already be valid C#, e.g.
        /// "new()", "\"literal\"" or "3f". Null for no initializer.
        /// </param>
        internal GeneratableField(string name, TypeRef fieldType, AccessModifier accessModifier, bool isStatic, string initialValueExpression = null)
            : base(name, accessModifier, isStatic)
        {
            this.fieldType = fieldType;
            this.initialValueExpression = initialValueExpression;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder field = new();

            field.Append(GetAttributesInline());
            field.Append(AccessModifier.AsString());
            PrependAdditionalLabels(field);
            if (IsStatic) field.Append(SPACE + STATIC);
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

        protected virtual void PrependAdditionalLabels(StringBuilder fieldStringBuilder) { }
    }

    /// <summary>
    /// A field whose type is a real compiled type, so its initial value can be supplied as a typed value
    /// rather than as a hand-written expression.
    /// </summary>
    public class GeneratableField<T> : GeneratableField
    {
        internal GeneratableField(string name, AccessModifier accessModifier, bool isStatic)
            : base(name, TypeRef.From(typeof(T)), accessModifier, isStatic) { }

        internal GeneratableField(string name, AccessModifier accessModifier, bool isStatic, T initialValue)
            : base(name, TypeRef.From(typeof(T)), accessModifier, isStatic, GetValueAsString(typeof(T), initialValue)) { }
    }
}
