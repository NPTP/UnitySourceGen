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
    /// TypeRef a = typeof(Vector2);            // "Vector2"
    /// TypeRef b = "PlayerActions";            // a type that is being generated right now
    /// TypeRef c = TypeRef.Generic("List", b); // "List&lt;PlayerActions&gt;"
    /// </code>
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

        public static TypeRef Void => new(VOID);

        public TypeRef(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? VOID : name.Trim();
        }

        /// <summary>
        /// Build from a real type, resolving keyword aliases, generic arguments, arrays and nullables.
        /// Namespaces are omitted, so the generated file needs a using directive for the type.
        /// </summary>
        public static TypeRef From(Type type) => new(FormatTypeName(type));

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

        public static implicit operator TypeRef(Type type) => From(type);
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
