using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A generated struct. Created with <see cref="SourceGen.NewStruct"/>; everything it can do is on
    /// <see cref="GeneratableTypeDefinition"/>.
    /// </summary>
    public sealed class GeneratableStruct : GeneratableTypeDefinition
    {
        protected override TypeDefinition TypeDefinition => TypeDefinition.Struct;

        internal GeneratableStruct(string name) : base(name) { }
    }
}
