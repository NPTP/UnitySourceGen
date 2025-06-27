using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Options;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public abstract class GeneratableProperty : GeneratableBase
    {
        protected GeneratableProperty(NameSyntax nameSyntax, AccessModifier accessModifier, bool isStatic) : base(nameSyntax, accessModifier, isStatic) { }
    }
}