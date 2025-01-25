using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableConstField<T> : GeneratableField<T>
    {
        private const string CONST = "const";
        
        internal GeneratableConstField(string name, AccessModifier accessModifier, T initialValue) : base(name, accessModifier, isStatic: false, initialValue) { }
        
        protected override void PrependAdditionalLabels(StringBuilder fieldStringBuilder)
        {
            fieldStringBuilder.Append(SPACE + CONST);
        }
    }
}