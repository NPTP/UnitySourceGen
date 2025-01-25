using NPTP.UnitySourceGen.Editor.Generatable;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class GeneratableEnumExtensions
    {
        public static GeneratableEnum AsFlags(this GeneratableEnum gen)
        {
            gen.IsFlags = true;
            return gen;
        }
        
        public static GeneratableEnum WithMember(this GeneratableEnum gen, string name)
        {
            gen.AddMember(name, GeneratableEnum.EnumMember.EnumValueMode.NonExplicit, null, null);
            return gen;
        }
        
        public static GeneratableEnum WithIntValuedMember(this GeneratableEnum gen, string name, int value)
        {
            gen.AddMember(name, GeneratableEnum.EnumMember.EnumValueMode.ExplicitInt, value, null);
            return gen;
        }
        
        public static GeneratableEnum WithBitShiftedMember(this GeneratableEnum gen, string name, int value, int bitShiftValue)
        {
            gen.AddMember(name, GeneratableEnum.EnumMember.EnumValueMode.ExplicitBitShiftFlag, value, bitShiftValue);
            return gen;
        }
    }
}
