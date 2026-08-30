using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableMethod : GeneratableBase
    {
        private const string EXPRESSION_ARROW = "=>";

        private readonly TypeRef returnType;
        private readonly GeneratableParameter[] parameters;
        private readonly GeneratableTypeParameter[] typeParameters;

        /// <summary>
        /// When true, <see cref="body"/> holds a single expression written after "=>" rather than the
        /// statements of a block body.
        /// </summary>
        private readonly bool isExpressionBodied;

        private readonly string[] body;

        /// <param name="isExpressionBodied">
        /// Pass true to write the method as "signature =&gt; expression;". The expression is taken from the
        /// first entry of <paramref name="body"/>. This is an explicit flag rather than a separate
        /// constructor overload, because a string overload would silently win over the params array for a
        /// single-line block body.
        /// </param>
        internal GeneratableMethod(string name, TypeRef returnType, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, bool isStatic,
            GeneratableParameter[] parameters, bool isExpressionBodied, params string[] body)
            : this(name, returnType, accessModifier, inheritanceModifier, isStatic, GeneratableTypeParameter.None, parameters, isExpressionBodied, body) { }

        internal GeneratableMethod(string name, TypeRef returnType, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, bool isStatic,
            GeneratableTypeParameter[] typeParameters, GeneratableParameter[] parameters, bool isExpressionBodied, params string[] body)
            : base(name, accessModifier, isStatic)
        {
            this.returnType = returnType;
            this.typeParameters = typeParameters ?? GeneratableTypeParameter.None;
            this.parameters = parameters ?? GeneratableParameter.None;
            this.isExpressionBodied = isExpressionBodied;
            this.body = body ?? Array.Empty<string>();
        }

        public override string GenerateStringRepresentation()
        {
            if (isExpressionBodied)
            {
                // Deliberately not AppendLine: a single-line member has no trailing newline, so it is not
                // mistaken for a block with an empty last line when split back into lines.
                string expression = body.Length > 0 ? body[0] : string.Empty;
                return $"{BuildSignature()} {EXPRESSION_ARROW} {expression}{SEMICOLON}";
            }

            int indent = 0;
            StringBuilder sb = new();

            AddLine(sb, indent, BuildSignature());
            AddOpenBrace(sb, indent);

            indent++;
            foreach (string line in body) AddLine(sb, indent, line);
            indent--;

            AddCloseBrace(sb, indent);

            return sb.ToString();
        }

        private string BuildSignature()
        {
            StringBuilder methodSignature = new();

            // TODO: rework hierarchy of classes so methods can have an inheritance modifier, partial, etc.

            methodSignature.Append(AccessModifier.AsString());
            if (IsStatic) methodSignature.Append(SPACE + STATIC);
            methodSignature.Append(SPACE + returnType.Name);
            methodSignature.Append(SPACE + Name);
            if (typeParameters.Length > 0)
            {
                methodSignature.Append("<" + string.Join(COMMA + SPACE, typeParameters.Select(typeParameter => typeParameter.Name)) + ">");
            }

            methodSignature.Append("(" + string.Join(COMMA + SPACE, parameters.Select(parameter => parameter.GetStringRepresentation())) + ")");

            foreach (GeneratableTypeParameter typeParameter in typeParameters.Where(typeParameter => typeParameter.HasConstraints))
            {
                methodSignature.Append(SPACE + typeParameter.GetConstraintClause());
            }

            return methodSignature.ToString();
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
            : base(name, TypeRef.From(typeof(T)), accessModifier, inheritanceModifier, isStatic, parameters, isExpressionBodied: false, body) { }
    }
}
