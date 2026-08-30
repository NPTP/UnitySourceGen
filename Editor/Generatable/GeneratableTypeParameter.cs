using System;
using System.Linq;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// One type parameter of a generic method, with its optional constraints.
    /// <code>
    /// GeneratableTypeParameter.Of("TDevice", "InputDevice")   // TDevice, where TDevice : InputDevice
    /// GeneratableTypeParameter.Of("T", Struct, "Enum")        // T, where T : struct, Enum
    /// GeneratableTypeParameter.Of("T")                        // T, unconstrained
    /// </code>
    /// Constraints are written in the order given, which is the caller's responsibility: C# requires the
    /// primary constraint (class / struct / a base class) first and new() last.
    /// </summary>
    public readonly struct GeneratableTypeParameter
    {
        /// <summary>No type parameters, i.e. a non-generic method.</summary>
        public static GeneratableTypeParameter[] None => Array.Empty<GeneratableTypeParameter>();

        // Keyword constraints, so callers do not have to remember the exact spelling.
        public static TypeRef Class => new("class");
        public static TypeRef Struct => new("struct");
        public static TypeRef NotNull => new("notnull");
        public static TypeRef Unmanaged => new("unmanaged");
        public static TypeRef New => new("new()");

        private readonly string name;
        private readonly TypeRef[] constraints;

        private GeneratableTypeParameter(string name, TypeRef[] constraints)
        {
            this.name = name;
            this.constraints = constraints ?? Array.Empty<TypeRef>();
        }

        public static GeneratableTypeParameter Of(string name, params TypeRef[] constraints) => new(name, constraints);

        internal string Name => name;

        internal bool HasConstraints => constraints is { Length: > 0 };

        internal TypeRef[] Constraints => constraints ?? Array.Empty<TypeRef>();

        /// <summary>e.g. "where TDevice : InputDevice". Empty when unconstrained.</summary>
        internal string GetConstraintClause()
        {
            return HasConstraints
                ? $"where {name} : {string.Join(", ", constraints.Select(constraint => constraint.Name))}"
                : string.Empty;
        }

        public override string ToString() => name;
    }
}
