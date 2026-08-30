using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A generated method, configured fluently on itself:
    /// <code>
    /// SourceGen.NewMethod("GetPlayer")
    ///     .Public().Static()
    ///     .Returning("InputPlayer")
    ///     .Taking(GeneratableParameter.Of&lt;int&gt;("playerID"))
    ///     .Expression("Runtime.GetPlayer(playerID)")
    /// </code>
    /// Defaults match C#: private, non-static, returning void, with an empty body.
    /// </summary>
    public class GeneratableMethod : GeneratableBase
    {
        private const string EXPRESSION_ARROW = "=>";

        private readonly List<GeneratableTypeParameter> typeParameters = new();
        private readonly List<GeneratableParameter> parameters = new();
        private readonly List<string> bodyLines = new();

        private TypeRef returnType = TypeRef.Void;

        /// <summary>
        /// When set, the method explicitly implements this interface's member, so it is written as
        /// "ReturnType IInterface.Name(...)" with no access modifier - explicit implementations may not
        /// have one, and are never static.
        /// </summary>
        private TypeRef explicitInterface;

        private InheritanceModifier inheritanceModifier = InheritanceModifier.None;
        private bool isExpressionBodied;

        internal GeneratableMethod(string name) : base(name, AccessModifier.Private, isStatic: false) { }

        /// <summary>
        /// Methods can be overloaded, so name alone is not enough to tell two of them apart. Parameter
        /// types and generic arity are what C# overload resolution uses, so they are what is compared here.
        /// </summary>
        internal override string DedupeKey =>
            $"{Name}`{typeParameters.Count}({string.Join(COMMA, parameters.Select(parameter => parameter.TypeName))})";

        #region Signature

        public GeneratableMethod Returning(TypeRef type)
        {
            returnType = type;
            return this;
        }

        public GeneratableMethod Returning<T>() => Returning(TypeRef.From(typeof(T)));

        public GeneratableMethod WithAccess(AccessModifier modifier)
        {
            AccessModifier = modifier;
            return this;
        }

        public GeneratableMethod Public() => WithAccess(AccessModifier.Public);
        public GeneratableMethod Private() => WithAccess(AccessModifier.Private);
        public GeneratableMethod Protected() => WithAccess(AccessModifier.Protected);
        public GeneratableMethod Internal() => WithAccess(AccessModifier.Internal);

        public GeneratableMethod Static()
        {
            IsStatic = true;
            return this;
        }

        public GeneratableMethod WithInheritanceModifier(InheritanceModifier modifier)
        {
            inheritanceModifier = modifier;
            return this;
        }

        /// <summary>
        /// Write this as an explicit interface implementation, e.g. "void IActionMapWrapper.Enable()".
        /// Explicit implementations are only reachable through the interface, so they keep the member off
        /// the type's own public surface. Any access modifier or static set here is ignored, since C# does
        /// not allow either on an explicit implementation.
        /// </summary>
        public GeneratableMethod ExplicitlyImplementing(TypeRef interfaceType)
        {
            explicitInterface = interfaceType;
            return this;
        }

        public GeneratableMethod ExplicitlyImplementing<T>() where T : class => ExplicitlyImplementing(TypeRef.From(typeof(T)));

        /// <summary>An attribute on the method, written on its own line above the signature.</summary>
        public GeneratableMethod WithAttribute(AddableAttribute attribute)
        {
            AddAttribute(attribute);
            return this;
        }

        public GeneratableMethod WithAttribute(string attributeName, params string[] arguments) =>
            WithAttribute(new AddableAttribute(attributeName, arguments));

        /// <summary>
        /// Wrap the method in "#if SYMBOL" / "#endif", e.g. OnlyIf("UNITY_EDITOR") for a member that must
        /// not reach a build.
        /// </summary>
        public GeneratableMethod OnlyIf(string conditionalCompilationSymbol)
        {
            ConditionalCompilationSymbol = conditionalCompilationSymbol;
            return this;
        }

        #endregion

        #region Generics And Parameters

        public GeneratableMethod Generic(params GeneratableTypeParameter[] methodTypeParameters)
        {
            if (methodTypeParameters != null) typeParameters.AddRange(methodTypeParameters);
            return this;
        }

        public GeneratableMethod Taking(params GeneratableParameter[] methodParameters)
        {
            if (methodParameters != null) parameters.AddRange(methodParameters);
            return this;
        }

        /// <summary>
        /// Makes this an extension method of the given type: the method becomes static and the "this"
        /// parameter is placed first, ahead of anything added with <see cref="Taking"/>.
        /// </summary>
        public GeneratableMethod Extending(TypeRef extendedType, string parameterName)
        {
            IsStatic = true;
            parameters.Insert(0, GeneratableParameter.Extension(extendedType, parameterName));
            return this;
        }

        #endregion

        #region Body

        /// <summary>A block body, one statement per line.</summary>
        public GeneratableMethod Body(params string[] lines)
        {
            isExpressionBodied = false;
            bodyLines.Clear();
            if (lines != null) bodyLines.AddRange(lines);
            return this;
        }

        /// <summary>An expression-bodied method: "signature =&gt; expression;".</summary>
        public GeneratableMethod Expression(string expression)
        {
            isExpressionBodied = true;
            bodyLines.Clear();
            bodyLines.Add(expression);
            return this;
        }

        #endregion

        public override string GenerateStringRepresentation()
        {
            int indent = 0;
            StringBuilder sb = new();

            AppendIfDirective(sb);
            AddAttributeLines(sb, indent);

            if (isExpressionBodied)
            {
                // Deliberately not AppendLine: a single-line member has no trailing newline, so it is not
                // mistaken for a block with an empty last line when split back into lines.
                string expression = bodyLines.Count > 0 ? bodyLines[0] : string.Empty;
                sb.Append($"{BuildSignature()} {EXPRESSION_ARROW} {expression}{SEMICOLON}");

                if (HasConditionalCompilation)
                {
                    sb.AppendLine();
                    AppendEndIfDirective(sb);
                }

                return sb.ToString();
            }

            AddLine(sb, indent, BuildSignature());
            AddOpenBrace(sb, indent);

            indent++;
            foreach (string line in bodyLines) AddLine(sb, indent, line);
            indent--;

            AddCloseBrace(sb, indent);
            AppendEndIfDirective(sb);

            return sb.ToString();
        }

        private string BuildSignature()
        {
            StringBuilder methodSignature = new();

            bool isExplicitImplementation = !string.IsNullOrEmpty(explicitInterface.Name) && !explicitInterface.IsVoid;

            if (isExplicitImplementation)
            {
                methodSignature.Append(returnType.Name);
                methodSignature.Append(SPACE + explicitInterface.Name + "." + Name);
            }
            else
            {
                methodSignature.Append(AccessModifier.AsString());
                if (IsStatic) methodSignature.Append(SPACE + STATIC);
                if (inheritanceModifier != InheritanceModifier.None) methodSignature.Append(SPACE + inheritanceModifier.AsString());
                methodSignature.Append(SPACE + returnType.Name);
                methodSignature.Append(SPACE + Name);
            }

            if (typeParameters.Count > 0)
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
}
