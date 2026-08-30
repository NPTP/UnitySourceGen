namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableEmptyMember : GeneratableBase
    {
        public GeneratableEmptyMember() : base(string.Empty, sanitizeName: false) { }

        public override string GenerateStringRepresentation()
        {
            return string.Empty;
        }
    }
}
