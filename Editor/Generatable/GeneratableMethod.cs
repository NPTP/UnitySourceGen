using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableMethod : GeneratableBase
    {
        private readonly TypeRef returnType;
        private readonly GeneratableParameter[] parameters;

        private IEnumerable<string> Body { get; }

        internal GeneratableMethod(string name, TypeRef returnType, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, bool isStatic,
            GeneratableParameter[] parameters, params string[] body)
            : base(name, accessModifier, isStatic)
        {
            this.returnType = returnType;
            this.parameters = parameters ?? GeneratableParameter.None;
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

            methodSignature.Append(AccessModifier.AsString());
            if (IsStatic) methodSignature.Append(SPACE + STATIC);
            methodSignature.Append(SPACE + returnType.Name);
            methodSignature.Append(SPACE + Name);
            methodSignature.Append("(" + string.Join(COMMA + SPACE, parameters.Select(parameter => parameter.GetStringRepresentation())) + ")");

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

    /// <summary>
    /// A method whose return type is a real compiled type. Use the non-generic
    /// <see cref="GeneratableMethod"/> to return a type that is itself being generated.
    /// </summary>
    public class GeneratableMethod<T> : GeneratableMethod
    {
        internal GeneratableMethod(string name, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, bool isStatic,
            GeneratableParameter[] parameters, params string[] body)
            : base(name, TypeRef.From(typeof(T)), accessModifier, inheritanceModifier, isStatic, parameters, body) { }
    }
}
