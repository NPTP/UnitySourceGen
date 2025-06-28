namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableComment : GeneratableBase
    {
        public GeneratableComment(string comment) : base(comment, default, default)
        {
        }

        public override string GenerateStringRepresentation() => Name; // Name == comment
    }
}
