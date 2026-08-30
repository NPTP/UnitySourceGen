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
    /// Defaults match C#: private, non-static, returning void, with an empty body. Because "void" is not
    /// a valid type argument, use <see cref="ReturningVoid"/> rather than Returning&lt;void&gt;().
    /// <para>
    /// The same type also covers constructors (<see cref="AsConstructor"/>), extension methods
    /// (<see cref="Extending"/>), explicit interface implementations
    /// (<see cref="ExplicitlyImplementing(Syntax.TypeRef)"/>), conversion operators
    /// (<see cref="AsImplicitConversion"/>), and generic methods with constraints
    /// (<see cref="Generic"/>).
    /// </para>
    /// </summary>
    public class GeneratableMethod : GeneratableBase
    {
        private const string EXPRESSION_ARROW = "=>";
        private const string IMPLICIT = "implicit";
        private const string EXPLICIT = "explicit";
        private const string OPERATOR = "operator";

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
        private bool isConstructor;
        private string baseConstructorArguments;
        private bool isExpressionBodied;

        /// <summary>
        /// When set, the method is written as a conversion operator - "public static implicit operator
        /// Name(...)" - whose target type is the method's own name.
        /// </summary>
        private string conversionKeyword;

        internal GeneratableMethod(string name) : base(name) { }

        /// <summary>
        /// The types this method names in its signature.
        /// </summary>
        internal override IEnumerable<TypeRef> ReferencedTypes
        {
            get
            {
                yield return returnType;
                foreach (GeneratableParameter parameter in parameters) yield return parameter.Type;
                foreach (GeneratableTypeParameter typeParameter in typeParameters)
                {
                    foreach (TypeRef constraint in typeParameter.Constraints) yield return constraint;
                }
            }
        }

        /// <summary>
        /// Methods can be overloaded, so name alone is not enough to tell two of them apart. Parameter
        /// types and generic arity are what C# overload resolution uses, so they are what is compared here.
        /// <para>
        /// A constructor, a conversion operator and a plain method can all share a name and parameter list
        /// while being different members, so what kind of member this is has to be part of the key too.
        /// </para>
        /// </summary>
        internal override string DedupeKey =>
            $"{DedupeKind}{Name}`{typeParameters.Count}({string.Join(COMMA, parameters.Select(parameter => parameter.TypeName))})";

        private string DedupeKind
        {
            get
            {
                if (isConstructor) return "ctor ";
                if (conversionKeyword != null) return conversionKeyword + SPACE + OPERATOR + SPACE;
                if (!string.IsNullOrEmpty(explicitInterface.Name) && !explicitInterface.IsVoid) return explicitInterface.Name + ".";
                return string.Empty;
            }
        }

        #region Signature

        public GeneratableMethod Returning(TypeRef type)
        {
            returnType = type;
            return this;
        }

        public GeneratableMethod Returning<T>() => Returning(TypeRef.From(typeof(T)));

        /// <summary>
        /// Returns void. This exists because "void" is not a valid generic type argument, so
        /// Returning&lt;void&gt;() cannot be written. Void is also the default, so this is for explicitness.
        /// </summary>
        public GeneratableMethod ReturningVoid() => Returning(TypeRef.Void);

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
        /// Write this as a constructor of the type it belongs to: no return type, and the name is the
        /// type's own name. Pass base constructor arguments to chain, e.g. ": base(id, asset)".
        /// </summary>
        public GeneratableMethod AsConstructor(string baseArguments = null)
        {
            isConstructor = true;
            baseConstructorArguments = baseArguments;
            return this;
        }

        /// <summary>
        /// Write this as an implicit conversion operator to the type this method is named after, e.g.
        /// SourceGen.NewMethod("InputPlayerRef").AsImplicitConversion() with a single parameter of the type
        /// being converted from. Conversion operators are always public and static, so any access modifier
        /// set here is ignored.
        /// </summary>
        public GeneratableMethod AsImplicitConversion()
        {
            conversionKeyword = IMPLICIT;
            return this;
        }

        /// <summary>As <see cref="AsImplicitConversion"/>, but requiring a cast at the call site.</summary>
        public GeneratableMethod AsExplicitConversion()
        {
            conversionKeyword = EXPLICIT;
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

        public GeneratableMethod Extending<T>(string parameterName) => Extending(TypeRef.From(typeof(T)), parameterName);

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

            if (isConstructor)
            {
                methodSignature.Append(AccessModifier.AsString());
                methodSignature.Append(SPACE + Name);
            }
            else if (conversionKeyword != null)
            {
                methodSignature.Append(AccessModifier.Public.AsString());
                methodSignature.Append(SPACE + STATIC);
                methodSignature.Append(SPACE + conversionKeyword);
                methodSignature.Append(SPACE + OPERATOR);
                methodSignature.Append(SPACE + Name);
            }
            else if (isExplicitImplementation)
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

            if (isConstructor && !string.IsNullOrEmpty(baseConstructorArguments))
            {
                methodSignature.Append(SPACE + ":" + SPACE + baseConstructorArguments);
            }

            foreach (GeneratableTypeParameter typeParameter in typeParameters.Where(typeParameter => typeParameter.HasConstraints))
            {
                methodSignature.Append(SPACE + typeParameter.GetConstraintClause());
            }

            return methodSignature.ToString();
        }
    }
}
