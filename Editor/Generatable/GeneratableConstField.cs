using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableConstField : GeneratableField
    {
        private const string CONST = "const";

        internal GeneratableConstField(string name, TypeRef fieldType, AccessModifier accessModifier, string initialValueExpression)
            : base(name, fieldType, accessModifier, isStatic: false, initialValueExpression) { }

        protected override void PrependAdditionalLabels(StringBuilder fieldStringBuilder)
        {
            fieldStringBuilder.Append(SPACE + CONST);
        }
    }

    public class GeneratableConstField<T> : GeneratableConstField
    {
        internal GeneratableConstField(string name, AccessModifier accessModifier, T initialValue)
            : base(name, TypeRef.From(typeof(T)), accessModifier, GetValueAsString(typeof(T), initialValue)) { }
    }
}
