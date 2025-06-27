using NPTP.UnitySourceGen.Editor.Extensions;

namespace NPTP.UnitySourceGen.Editor.Options
{
    public struct NameSyntax
    {
        public string name;
        public string prefix;
        public string suffix;

        public bool IsValid() => !string.IsNullOrEmpty(name);
        public string GetName() => $"{prefix.StringValueOrEmpty()}{name.StringValueOrEmpty()}{suffix.StringValueOrEmpty()}";

        public NameSyntax WithLowerCaseName()
        {
            NameSyntax withLowerCase = this;
            withLowerCase.name = withLowerCase.name.LowercaseFirst();
            return withLowerCase;
        }
        
        public NameSyntax WithUpperCaseName()
        {
            NameSyntax withUpperCase = this;
            withUpperCase.name = withUpperCase.name.UppercaseFirst();
            return withUpperCase;
        }

        public static implicit operator NameSyntax(string name) => new() { name = name };
    }
}
