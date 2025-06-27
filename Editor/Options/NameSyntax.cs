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

        public static implicit operator NameSyntax(string name) => new() { name = name };
    }
}
