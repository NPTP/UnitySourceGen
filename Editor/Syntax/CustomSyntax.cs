using NPTP.UnitySourceGen.Editor.Extensions;
using NPTP.UnitySourceGen.Editor.Extensions.Internal;

namespace NPTP.UnitySourceGen.Editor.Syntax
{
    public struct CustomSyntax
    {
        public string prefix;
        public string suffix;

        public static CustomSyntax Default => new CustomSyntax();

        public readonly string InSyntax(string s)
        {
            return $"{prefix.StringValueOrEmpty()}" +
                   $"{s}" +
                   $"{suffix.StringValueOrEmpty()}";
        }
    }
}
