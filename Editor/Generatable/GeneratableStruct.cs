using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public sealed class GeneratableStruct : GeneratableTypeDefinition
    {
        protected override TypeDefinition TypeDefinition => TypeDefinition.Struct;

        internal GeneratableStruct(string nameSyntax, AccessModifier accessModifier) : base(nameSyntax, accessModifier, isStatic: false)
        {
        }
    }
}
