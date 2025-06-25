namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableEmptyMember : GeneratableBase
    {
        public GeneratableEmptyMember() : base(default, default, default) { }

        public override string GenerateStringRepresentation()
        {
            return string.Empty;
        }
    }
}
