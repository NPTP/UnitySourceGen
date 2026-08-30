using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A generated class. Created with <see cref="SourceGen.NewClass"/> or
    /// <see cref="SourceGen.NewStaticClass"/>; everything it can do is on
    /// <see cref="GeneratableTypeDefinition"/>.
    /// </summary>
    public sealed class GeneratableClass : GeneratableTypeDefinition
    {
        protected override TypeDefinition TypeDefinition => TypeDefinition.Class;

        internal GeneratableClass(string name, AccessModifier accessModifier, bool isStatic) : base(name, accessModifier, isStatic)
        {
        }
    }
}
