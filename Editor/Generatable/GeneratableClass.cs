using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A generated class. Created with <see cref="SourceGen.NewClass"/>; everything it can do is on
    /// <see cref="GeneratableTypeDefinition"/>.
    /// </summary>
    public sealed class GeneratableClass : GeneratableTypeDefinition
    {
        protected override TypeDefinition TypeDefinition => TypeDefinition.Class;

        internal GeneratableClass(string name) : base(name) { }
    }
}
