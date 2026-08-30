using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// One parameter in a generated method signature.
    /// <code>
    /// GeneratableParameter.Of("int", "playerID")                        // int playerID
    /// GeneratableParameter.Of&lt;int&gt;("playerID", defaultValue: "0")      // int playerID = 0
    /// GeneratableParameter.Extension("InputPlayer", "inputPlayer")      // this InputPlayer inputPlayer
    /// GeneratableParameter.Out("ActionWrapper", "actionWrapper")        // out ActionWrapper actionWrapper
    /// </code>
    /// </summary>
    public readonly struct GeneratableParameter
    {
        /// <summary>An empty parameter list.</summary>
        public static GeneratableParameter[] None => System.Array.Empty<GeneratableParameter>();

        private readonly TypeRef parameterType;
        private readonly string name;
        private readonly ParameterModifier modifier;

        /// <summary>Written verbatim after "=", so it must already be valid C#. Null for no default.</summary>
        private readonly string defaultValueExpression;

        private GeneratableParameter(TypeRef parameterType, string name, ParameterModifier modifier, string defaultValueExpression)
        {
            this.parameterType = parameterType;
            this.name = name;
            this.modifier = modifier;
            this.defaultValueExpression = defaultValueExpression;
        }

        public static GeneratableParameter Of(TypeRef parameterType, string name, string defaultValue = null) =>
            new(parameterType, name, ParameterModifier.None, defaultValue);

        public static GeneratableParameter Of<T>(string name, string defaultValue = null) =>
            new(TypeRef.From(typeof(T)), name, ParameterModifier.None, defaultValue);

        /// <summary>The "this" parameter that makes a static method an extension method.</summary>
        public static GeneratableParameter Extension(TypeRef parameterType, string name) =>
            new(parameterType, name, ParameterModifier.This, null);

        public static GeneratableParameter Extension<T>(string name) =>
            new(TypeRef.From(typeof(T)), name, ParameterModifier.This, null);

        public static GeneratableParameter Out(TypeRef parameterType, string name) =>
            new(parameterType, name, ParameterModifier.Out, null);

        public static GeneratableParameter Ref(TypeRef parameterType, string name) =>
            new(parameterType, name, ParameterModifier.Ref, null);

        public static GeneratableParameter In(TypeRef parameterType, string name) =>
            new(parameterType, name, ParameterModifier.In, null);

        public static GeneratableParameter Params(TypeRef arrayParameterType, string name) =>
            new(arrayParameterType, name, ParameterModifier.Params, null);

        /// <summary>The parameter''s type name, used to tell overloads apart.</summary>
        internal string TypeName => parameterType.Name;

        public string GetStringRepresentation()
        {
            string modifierText = modifier.AsString();
            string prefix = string.IsNullOrEmpty(modifierText) ? string.Empty : modifierText + " ";
            string suffix = defaultValueExpression == null ? string.Empty : " = " + defaultValueExpression;
            return $"{prefix}{parameterType.Name} {name}{suffix}";
        }

        public override string ToString() => GetStringRepresentation();
    }
}
