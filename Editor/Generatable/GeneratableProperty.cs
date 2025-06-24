using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public abstract class GeneratableProperty : GeneratableBase
    {
        protected GeneratableProperty(string name, AccessModifier accessModifier, bool isStatic) : base(name, accessModifier, isStatic) { }
    }
}