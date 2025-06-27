using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Options;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableConstField<T> : GeneratableField<T>
    {
        private const string CONST = "const";
        
        internal GeneratableConstField(NameSyntax nameSyntax, AccessModifier accessModifier, T initialValue) : base(nameSyntax, accessModifier, isStatic: false, initialValue) { }
        
        protected override void PrependAdditionalLabels(StringBuilder fieldStringBuilder)
        {
            fieldStringBuilder.Append(SPACE + CONST);
        }
    }
}