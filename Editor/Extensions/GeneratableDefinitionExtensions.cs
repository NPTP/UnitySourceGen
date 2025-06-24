using NPTP.UnitySourceGen.Editor.Generatable;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class GeneratableDefinitionExtensions
    {
        public static T InNamespace<T>(this T gen, string @namespace) where T : GeneratableDefinition
        {
            gen.Namespace = @namespace;
            return gen;
        }

        /// <summary>
        /// Write like: WithDirective("UnityEngine"), rather than WithDirective("using UnityEngine;").
        /// </summary>
        public static T WithDirective<T>(this T gen, string directive) where T : GeneratableDefinition
        {
            gen.Directives.Add(directive);
            return gen;
        }

        public static T WithDirectives<T>(this T gen, params string[] directives) where T : GeneratableDefinition
        {
            foreach (string directive in directives)
            {
                gen.Directives.Add(directive);
            }

            return gen;
        }
    }
}
