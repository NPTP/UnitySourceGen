namespace NPTP.UnitySourceGen.Editor.Generatable.Directives
{
    public class Directive
    {
        private readonly string content;
        
        public Directive(string content)
        {
            this.content = content;
        }
        
        public override string ToString() => $"using {content};";
        public bool Matches(string line) => line.Contains(ToString());

        public static implicit operator string(Directive directive) => directive.ToString();
    }
}
