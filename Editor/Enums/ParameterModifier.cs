using System;

namespace NPTP.UnitySourceGen.Editor.Enums
{
    public enum ParameterModifier
    {
        None = 0,

        /// <summary>Makes the containing static method an extension method.</summary>
        This,

        Ref,
        Out,
        In,
        Params
    }

    public static class ParameterModifierExtensions
    {
        public static string AsString(this ParameterModifier parameterModifier)
        {
            return parameterModifier switch
            {
                ParameterModifier.None => string.Empty,
                ParameterModifier.This => "this",
                ParameterModifier.Ref => "ref",
                ParameterModifier.Out => "out",
                ParameterModifier.In => "in",
                ParameterModifier.Params => "params",
                _ => throw new ArgumentOutOfRangeException(nameof(parameterModifier), parameterModifier, null)
            };
        }
    }
}
