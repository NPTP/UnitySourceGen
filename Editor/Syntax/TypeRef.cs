using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPTP.UnitySourceGen.Editor.Syntax
{
    /// <summary>
    /// The name of a type as it should appear in generated code.
    /// <para>
    /// Generated code frequently has to name types that do not exist yet - other types being generated in
    /// the same run - so a type cannot always be supplied as a <see cref="Type"/>. A TypeRef can be built
    /// from either a real type or a raw string, and both convert implicitly:
    /// </para>
    /// <code>
    /// TypeRef a = "PlayerActions";              // a type that is being generated right now
    /// TypeRef b = TypeRef.From(typeof(Vector2)); // a type that already exists
    /// TypeRef c = TypeRef.Generic("List", a);    // "List&lt;PlayerActions&gt;"
    /// </code>
    /// Names are sanitized: identifiers inside them are made valid, while the punctuation that shapes
    /// generics, arrays and nullables is preserved.
    /// <para>
    /// Strings convert implicitly, so a generated type name can be passed anywhere a TypeRef is taken.
    /// An existing type does not: the generic overloads - NewField&lt;T&gt;, Returning&lt;T&gt; and the
    /// rest - are the way to name one, so there is never a second redundant signature taking a typeof.
    /// </para>
    /// A TypeRef built from a real type also knows its <see cref="Namespace"/>, which is what lets a
    /// generated file work out its own using directives. One built from a string cannot, so that directive
    /// has to be added by hand with WithDirective.
    /// </summary>
    public readonly struct TypeRef : IEquatable<TypeRef>
    {
        private const string VOID = "void";

        /// <summary>
        /// C# keyword aliases. Using these rather than the CLR type names keeps generated code idiomatic.
        /// </summary>
        private static readonly Dictionary<Type, string> keywordAliases = new()
        {
            { typeof(void), VOID },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(nint), "nint" },
            { typeof(nuint), "nuint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(string), "string" },
            { typeof(object), "object" }
        };

        /// <summary>The type name exactly as it will be written into the generated file.</summary>
        public string Name { get; }

        /// <summary>
        /// The namespace the type lives in, when it is known. Only types built from a real
        /// <see cref="Type"/> know theirs - a TypeRef made from a raw string has no way to, so the using
        /// directive for it has to be added by hand.
        /// </summary>
        public string Namespace { get; }

        public static TypeRef Void => new(VOID);

        public TypeRef(string name) : this(name, null) { }

        private TypeRef(string name, string typeNamespace)
        {
            // Type names are frequently built from asset names, so the identifiers inside them are
            // sanitized here. The punctuation that shapes the name - generics, arrays, nullables - is left
            // intact, and a name coming from a real Type is already valid so nothing changes.
            Name = string.IsNullOrWhiteSpace(name) ? VOID : GeneratedIdentifier.SanitizeTypeName(name.Trim());
            Namespace = typeNamespace;
        }

        /// <summary>
        /// Build from a real type, resolving keyword aliases, generic arguments, arrays and nullables.
        /// Namespaces are omitted, so the generated file needs a using directive for the type.
        /// </summary>
        public static TypeRef From(Type type) => new(FormatTypeName(type), type?.Namespace);

        /// <summary>e.g. Generic("Dictionary", "string", "PlayerActions") -> Dictionary&lt;string, PlayerActions&gt;</summary>
        public static TypeRef Generic(string genericTypeName, params TypeRef[] typeArguments)
        {
            if (typeArguments == null || typeArguments.Length == 0)
            {
                return new TypeRef(genericTypeName);
            }

            return new TypeRef($"{StripArity(genericTypeName)}<{string.Join(", ", typeArguments.Select(argument => argument.Name))}>");
        }

        /// <summary>e.g. Array("BindingInfo") -> BindingInfo[]</summary>
        public static TypeRef Array(TypeRef elementType) => new(elementType.Name + "[]");

        /// <summary>e.g. Nullable("int") -> int?</summary>
        public static TypeRef Nullable(TypeRef underlyingType) => new(underlyingType.Name + "?");

        public bool IsVoid => Name == VOID;

        private static string FormatTypeName(Type type)
        {
            if (type == null)
            {
                return VOID;
            }

            if (keywordAliases.TryGetValue(type, out string alias))
            {
                return alias;
            }

            if (type.IsArray)
            {
                return FormatTypeName(type.GetElementType()) + "[]";
            }

            Type underlyingNullableType = System.Nullable.GetUnderlyingType(type);
            if (underlyingNullableType != null)
            {
                return FormatTypeName(underlyingNullableType) + "?";
            }

            if (!type.IsGenericType)
            {
                // Nested types come through as "Outer+Inner".
                return type.Name.Replace('+', '.');
            }

            string genericArguments = string.Join(", ", type.GetGenericArguments().Select(FormatTypeName));
            return $"{StripArity(type.Name.Replace('+', '.'))}<{genericArguments}>";
        }

        /// <summary>Reflection reports generic type names as e.g. "ValueActionWrapper`1".</summary>
        private static string StripArity(string typeName)
        {
            int backtickIndex = typeName.IndexOf('`');
            return backtickIndex < 0 ? typeName : typeName.Substring(0, backtickIndex);
        }

        // Deliberately no implicit conversion from Type: that would duplicate the generic overloads,
        // which are the one way to name a type that already exists. Strings name types being generated.
        public static implicit operator TypeRef(string typeName) => new(typeName);
        public static implicit operator string(TypeRef typeRef) => typeRef.Name;

        public override string ToString() => Name;

        public bool Equals(TypeRef other) => Name == other.Name;
        public override bool Equals(object obj) => obj is TypeRef other && Equals(other);
        public override int GetHashCode() => Name == null ? 0 : Name.GetHashCode();
        public static bool operator ==(TypeRef a, TypeRef b) => a.Equals(b);
        public static bool operator !=(TypeRef a, TypeRef b) => !a.Equals(b);
    }
}
