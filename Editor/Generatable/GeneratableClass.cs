using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public sealed class GeneratableClass : GeneratableTypeDefinition
    {
        protected override TypeDefinition TypeDefinition => TypeDefinition.Class;
        
        internal GeneratableClass(string name, AccessModifier accessModifier, bool isStatic) : base(name, accessModifier, isStatic)
        {
        }
    }
}
