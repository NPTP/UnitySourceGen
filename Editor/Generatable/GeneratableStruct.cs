using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public sealed class GeneratableStruct : GeneratableTypeDefinition
    {
        protected override TypeDefinition TypeDefinition => TypeDefinition.Struct;

        internal GeneratableStruct(string name, AccessModifier accessModifier) : base(name, accessModifier, isStatic: false)
        {
        }
    }
}
