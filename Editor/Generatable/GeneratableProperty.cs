using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableProperty : GeneratableBase
    {
        internal GeneratableProperty(string name, AccessModifier getModifier, AccessModifier setModifier, bool isStatic) : base(name, getModifier, isStatic)
        {
        }

        public override string GenerateStringRepresentation()
        {
            throw new System.NotImplementedException();
        }
    }
}