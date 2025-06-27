using System.Collections.Generic;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Options;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public abstract class GeneratableMethod : GeneratableBase
    {
        protected GeneratableMethod(NameSyntax nameSyntax, AccessModifier accessModifier, bool isStatic) : base(nameSyntax, accessModifier, isStatic) { }
    }
    
    public class GeneratableMethod<T> : GeneratableMethod
    {
        private IEnumerable<string> Body { get; }
        
        // TODO: Support parameters
        
        internal GeneratableMethod(NameSyntax nameSyntax, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, bool isStatic, params string[] body) : base(nameSyntax, accessModifier, isStatic)
        {
            Body = body;
        }
        
        public override string GenerateStringRepresentation()
        {
            int indent = 0;
            StringBuilder sb = new();
            
            AddMethodSignature(sb, indent);
            AddOpenBrace(sb, indent);
            
            indent++;
            AddBody(sb, indent);
            indent--;
            
            AddCloseBrace(sb, indent);
            
            return sb.ToString();
        }

        private void AddMethodSignature(StringBuilder sb, int indent)
        {
            StringBuilder methodSignature = new();
            
            // TODO: rework hierarchy of classes so methods can have an inheritance modifier, partial, etc.
            // string inheritanceModifier = ""; // InheritanceModifier.AsString();
            // string partial = ""; // IsPartial ? "partial" : string.Empty;
            
            // if (IsPartial) methodSignature.Append(SPACE + partial);
            methodSignature.Append(AccessModifier.AsString());
            if (IsStatic) methodSignature.Append(SPACE + STATIC);
            // if (InheritanceModifier != InheritanceModifier.None) methodSignature.Append(SPACE + inheritanceModifier);
            methodSignature.Append(SPACE + GetTypeName(typeof(T)));
            methodSignature.Append(SPACE + Name);
            methodSignature.Append("()");
            
            AddLine(sb, indent, methodSignature.ToString());
        }

        private void AddBody(StringBuilder sb, int indent)
        {
            foreach (string line in Body)
            {
                AddLine(sb, indent, line);
            }
        }
    }
}