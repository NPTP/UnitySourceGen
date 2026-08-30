using System.Collections.Generic;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// Configures one generated method. Every aspect of a method - return type, access, static, generics,
    /// parameters, block or expression body - is set here rather than through a separate extension overload
    /// for each combination.
    /// <code>
    /// gen.WithMethod("GetPlayer", m =&gt; m
    ///     .Public().Static()
    ///     .Returning("InputPlayer")
    ///     .Taking(GeneratableParameter.Of&lt;int&gt;("playerID"))
    ///     .Expression("Runtime.GetPlayer(playerID)"));
    /// </code>
    /// Defaults match C#: private, non-static, returning void, with an empty body.
    /// </summary>
    public class GeneratableMethodBuilder
    {
        private readonly string name;
        private readonly List<GeneratableTypeParameter> typeParameters = new();
        private readonly List<GeneratableParameter> parameters = new();
        private readonly List<string> bodyLines = new();
        private readonly List<AddableAttribute> attributes = new();

        private TypeRef returnType = TypeRef.Void;
        private TypeRef explicitInterface;
        private AccessModifier accessModifier = AccessModifier.Private;
        private InheritanceModifier inheritanceModifier = InheritanceModifier.None;
        private bool isStatic;
        private bool isExpressionBodied;
        private string conditionalCompilationSymbol;

        internal GeneratableMethodBuilder(string name)
        {
            this.name = name;
        }

        #region Signature

        public GeneratableMethodBuilder Returning(TypeRef type)
        {
            returnType = type;
            return this;
        }

        public GeneratableMethodBuilder Returning<T>() => Returning(TypeRef.From(typeof(T)));

        public GeneratableMethodBuilder WithAccess(AccessModifier modifier)
        {
            accessModifier = modifier;
            return this;
        }

        public GeneratableMethodBuilder Public() => WithAccess(AccessModifier.Public);
        public GeneratableMethodBuilder Private() => WithAccess(AccessModifier.Private);
        public GeneratableMethodBuilder Protected() => WithAccess(AccessModifier.Protected);
        public GeneratableMethodBuilder Internal() => WithAccess(AccessModifier.Internal);

        /// <summary>An attribute on the method, written on its own line above the signature.</summary>
        public GeneratableMethodBuilder WithAttribute(AddableAttribute attribute)
        {
            if (attribute != null) attributes.Add(attribute);
            return this;
        }

        public GeneratableMethodBuilder WithAttribute(string attributeName, params string[] arguments) =>
            WithAttribute(new AddableAttribute(attributeName, arguments));

        /// <summary>
        /// Wrap the method in "#if SYMBOL" / "#endif", e.g. OnlyIf("UNITY_EDITOR") for a member that must
        /// not reach a build.
        /// </summary>
        public GeneratableMethodBuilder OnlyIf(string conditionalCompilationSymbol)
        {
            this.conditionalCompilationSymbol = conditionalCompilationSymbol;
            return this;
        }

        public GeneratableMethodBuilder Static()
        {
            isStatic = true;
            return this;
        }

        public GeneratableMethodBuilder WithInheritanceModifier(InheritanceModifier modifier)
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
        public GeneratableMethodBuilder ExplicitlyImplementing(TypeRef interfaceType)
        {
            explicitInterface = interfaceType;
            return this;
        }

        public GeneratableMethodBuilder ExplicitlyImplementing<T>() where T : class => ExplicitlyImplementing(TypeRef.From(typeof(T)));

        #endregion

        #region Generics And Parameters

        public GeneratableMethodBuilder Generic(params GeneratableTypeParameter[] methodTypeParameters)
        {
            if (methodTypeParameters != null) typeParameters.AddRange(methodTypeParameters);
            return this;
        }

        public GeneratableMethodBuilder Taking(params GeneratableParameter[] methodParameters)
        {
            if (methodParameters != null) parameters.AddRange(methodParameters);
            return this;
        }

        /// <summary>
        /// Makes this an extension method of the given type: the method becomes static and the "this"
        /// parameter is placed first, ahead of anything added with <see cref="Taking"/>.
        /// </summary>
        public GeneratableMethodBuilder Extending(TypeRef extendedType, string parameterName)
        {
            isStatic = true;
            parameters.Insert(0, GeneratableParameter.Extension(extendedType, parameterName));
            return this;
        }

        #endregion

        #region Body

        /// <summary>A block body, one statement per line.</summary>
        public GeneratableMethodBuilder Body(params string[] lines)
        {
            isExpressionBodied = false;
            bodyLines.Clear();
            if (lines != null) bodyLines.AddRange(lines);
            return this;
        }

        /// <summary>An expression-bodied method: "signature =&gt; expression;".</summary>
        public GeneratableMethodBuilder Expression(string expression)
        {
            isExpressionBodied = true;
            bodyLines.Clear();
            bodyLines.Add(expression);
            return this;
        }

        #endregion

        internal GeneratableMethod Build()
        {
            GeneratableMethod method = new(name, returnType, accessModifier, inheritanceModifier, isStatic,
                typeParameters.ToArray(), parameters.ToArray(), explicitInterface, isExpressionBodied, bodyLines.ToArray());

            foreach (AddableAttribute attribute in attributes) method.AddAttribute(attribute);
            method.ConditionalCompilationSymbol = conditionalCompilationSymbol;

            return method;
        }
    }
}
