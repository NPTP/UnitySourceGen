using System;

namespace NPTP.UnitySourceGen.Editor.Generatable.Directives
{
    public class Alias
    {
        private readonly string alias;
        private readonly Type originalType;
        
        public Alias(string alias, Type originalType)
        {
            this.alias = alias;
            this.originalType = originalType;
        }
        
        public override string ToString() => $"using {alias} = {originalType.FullName};";
        public bool Matches(string line) => line.Contains(ToString());

        public static implicit operator string(Alias alias) => alias.ToString();
    }
}
