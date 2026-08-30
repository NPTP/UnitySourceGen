namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A single-line comment. Owns the "//" prefix, so callers pass the text alone.
    /// </summary>
    public class GeneratableComment : GeneratableBase
    {
        private const string PREFIX = "// ";

        public GeneratableComment(string comment) : base(comment, default, default) { }

        public override string GenerateStringRepresentation() => PREFIX + Name;

        /// <summary>True if the line already contains this comment, used to avoid duplicating it.</summary>
        public bool Matches(string line) => line != null && line.Contains(GenerateStringRepresentation());

        public static implicit operator string(GeneratableComment comment) => comment.GenerateStringRepresentation();
    }
}
