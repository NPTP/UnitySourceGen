# Unity Source Gen
## Changelog

1.1.1
- Tell constructors, conversion operators, explicit implementations and plain methods apart when deduping, so members sharing a name and parameter list no longer replace each other

1.1.0
- Conversion operators: `AsImplicitConversion` and `AsExplicitConversion` on a method, whose name is the type converted to
- `ReadOnly` on a struct

1.0.1
- Add `GeneratedIdentifier.SanitizeAsPascalCase`, the counterpart to the existing camelCase overload

1.0.0
- Fluent API throughout: everything is created from `SourceGen` with a name and configured on itself. Builder and extension classes removed
- Constructors take only what is required; a field or property takes its type, everything else has a C# default
- Names are sanitized into valid identifiers automatically, including type names, which are sanitized per identifier so generics, arrays and nullables survive
- `TypeRef` can name types that do not exist yet, so generated types can reference each other. Handles generics, arrays and nullables
- Methods support parameters, generic type parameters with constraints, extension methods, expression bodies, constructors and explicit interface implementations
- Events, properties with accessor bodies, and attributes with arguments
- Multiple types and namespaces per file, with using directives collected automatically
- Conditional compilation blocks on files, types and methods
- Write any generatable to any path in Assets; unchanged files are skipped rather than rewritten, and nothing is logged per file
- Duplicate members are detected by signature, so overloads survive; a colliding member replaces the existing one

0.1.6
- Make some methods `internal` which shouldn't have been visible to other assemblies

0.1.5
- New features added to code chunks and modifiable scripts
- Serialized property custom syntax fixed

0.1.4
- Refactor how to modify code in existing script, cleaner syntax and less steps

0.1.3
- Support custom naming syntax with prefix/suffix
- Support writing to a named #region in an existing script
- Support adding directives & aliases to an existing script

0.1.2
- CodeChunk generation implemented
- Replace code in specifically marked parts of existing scripts

0.1.1
- SerializedProperty generation
- Write to file directly from SourceGen

0.1.0
- Fix a few bugs
- Can now write directly to files & replace specific classes in script files

