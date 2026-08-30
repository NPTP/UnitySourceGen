using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A generated class or struct, configured fluently on itself:
    /// <code>
    /// SourceGen.NewClass("ISW").Public().Static()
    ///     .InNamespace("MyGame")
    ///     .WithDirectives("System", "UnityEngine")
    ///     .WithMethod(SourceGen.NewMethod("Initialize").Private().Static().Body("..."))
    /// </code>
    /// A type with no access modifier set is internal, as in C#. Members are added with WithField,
    /// WithEvent, WithProperty and WithMethod; adding one that collides with an existing member replaces
    /// it, so a default can be set first and overridden later.
    /// </summary>
    public abstract class GeneratableTypeDefinition : GeneratableDefinition
    {
        private const string PARTIAL = "partial";

        protected abstract TypeDefinition TypeDefinition { get; }

        private InheritanceModifier inheritanceModifier;
        private bool isPartial;
        private bool isReadOnly;
        private string baseClassTypeName;
        private SortedSet<string> ImplementsInterfaces { get; } = new();

        private List<GeneratableField> Fields { get; } = new();
        private List<GeneratableEvent> Events { get; } = new();
        private List<GeneratableProperty> Properties { get; } = new();
        private List<GeneratableMethod> Methods { get; } = new();

        internal GeneratableTypeDefinition(string name) : base(name) { }

        #region File Placement

        public new GeneratableTypeDefinition InNamespace(string @namespace)
        {
            Namespace = @namespace;
            return this;
        }

        /// <summary>Write like WithDirective("UnityEngine"), rather than WithDirective("using UnityEngine;").</summary>
        public new GeneratableTypeDefinition WithDirective(string directive)
        {
            Directives.Add(directive);
            return this;
        }

        public new GeneratableTypeDefinition WithDirectives(params string[] directives)
        {
            if (directives != null)
            {
                foreach (string directive in directives) Directives.Add(directive);
            }

            return this;
        }

        #endregion

        #region Declaration

        public GeneratableTypeDefinition WithAccess(AccessModifier modifier)
        {
            AccessModifier = modifier;
            return this;
        }

        public GeneratableTypeDefinition Public() => WithAccess(AccessModifier.Public);
        public GeneratableTypeDefinition Private() => WithAccess(AccessModifier.Private);
        public GeneratableTypeDefinition Protected() => WithAccess(AccessModifier.Protected);
        public GeneratableTypeDefinition Internal() => WithAccess(AccessModifier.Internal);

        public GeneratableTypeDefinition Static()
        {
            IsStatic = true;
            return this;
        }

        public GeneratableTypeDefinition WithInheritanceModifier(InheritanceModifier modifier)
        {
            inheritanceModifier = modifier;
            return this;
        }

        /// <summary>
        /// A readonly struct, whose fields the compiler enforces as immutable. Has no meaning on a class.
        /// </summary>
        public GeneratableTypeDefinition ReadOnly()
        {
            isReadOnly = true;
            return this;
        }

        public GeneratableTypeDefinition AsPartial()
        {
            isPartial = true;
            return this;
        }

        public GeneratableTypeDefinition InheritsFrom(TypeRef baseType)
        {
            baseClassTypeName = baseType.Name;
            return this;
        }

        public GeneratableTypeDefinition InheritsFrom<T>() => InheritsFrom(TypeRef.From(typeof(T)));

        public GeneratableTypeDefinition ImplementsInterface(TypeRef interfaceType)
        {
            ImplementsInterfaces.Add(interfaceType.Name);
            return this;
        }

        public GeneratableTypeDefinition ImplementsInterface<T>() where T : class => ImplementsInterface(TypeRef.From(typeof(T)));

        /// <summary>An attribute on the type itself, written on its own line above the signature.</summary>
        public GeneratableTypeDefinition WithAttribute(AddableAttribute attribute)
        {
            AddAttribute(attribute);
            return this;
        }

        public GeneratableTypeDefinition WithAttribute(string attributeName, params string[] arguments) =>
            WithAttribute(new AddableAttribute(attributeName, arguments));

        /// <summary>Wrap the whole type in "#if SYMBOL" / "#endif".</summary>
        public GeneratableTypeDefinition OnlyIf(string conditionalCompilationSymbol)
        {
            ConditionalCompilationSymbol = conditionalCompilationSymbol;
            return this;
        }

        #endregion

        #region Members

        public GeneratableTypeDefinition WithField(GeneratableField field)
        {
            Add(field, Fields);
            return this;
        }

        public GeneratableTypeDefinition WithEvent(GeneratableEvent generatableEvent)
        {
            Add(generatableEvent, Events);
            return this;
        }

        public GeneratableTypeDefinition WithProperty(GeneratableProperty property)
        {
            Add(property, Properties);
            return this;
        }

        public GeneratableTypeDefinition WithMethod(GeneratableMethod method)
        {
            Add(method, Methods);
            return this;
        }

        public GeneratableTypeDefinition WithMethods(params GeneratableMethod[] methods)
        {
            if (methods != null)
            {
                foreach (GeneratableMethod method in methods) Add(method, Methods);
            }

            return this;
        }

        #endregion

        /// <summary>
        /// The namespaces of every type named by this type's members, so the containing file can add the
        /// using directives they need. Types named as raw strings are not included: a string carries no
        /// namespace, so those directives still have to be added by hand.
        /// </summary>
        internal IEnumerable<string> GetRequiredNamespaces()
        {
            foreach (GeneratableBase member in Fields.Cast<GeneratableBase>().Concat(Events).Concat(Properties).Concat(Methods))
            {
                foreach (TypeRef referencedType in member.ReferencedTypes)
                {
                    if (!string.IsNullOrEmpty(referencedType.Namespace)) yield return referencedType.Namespace;
                }
            }
        }

        internal override void AppendTypeDeclaration(StringBuilder sb, int indent)
        {
            AppendIfDirective(sb);
            AddAttributeLines(sb, indent);
            AddClassSignature(sb, indent);
            AddOpenBrace(sb, indent);

            indent++;

            AddFields(sb, indent);
            AddEvents(sb, indent);
            AddProperties(sb, indent);
            AddMethods(sb, indent);

            indent--;

            AddCloseBrace(sb, indent);

            if (HasConditionalCompilation)
            {
                AppendEndIfDirective(sb);
                sb.AppendLine();
            }
        }

        private void AddClassSignature(StringBuilder sb, int indent)
        {
            StringBuilder classSignature = new();

            classSignature.Append(AccessModifier.AsString());
            if (IsStatic) classSignature.Append(SPACE + STATIC);
            if (inheritanceModifier != InheritanceModifier.None) classSignature.Append(SPACE + inheritanceModifier.AsString());
            if (isReadOnly) classSignature.Append(SPACE + READONLY);
            if (isPartial) classSignature.Append(SPACE + PARTIAL);
            classSignature.Append(SPACE + TypeDefinition.AsString());
            classSignature.Append(SPACE + Name);

            bool inheritsFromSomething = !string.IsNullOrEmpty(baseClassTypeName);
            bool implementsInterfaces = ImplementsInterfaces.Count > 0;
            if (inheritsFromSomething || implementsInterfaces)
            {
                classSignature.Append(SPACE + ":" + SPACE);
                if (inheritsFromSomething)
                {
                    classSignature.Append(baseClassTypeName);
                    if (implementsInterfaces) classSignature.Append(COMMA + SPACE);
                }

                if (implementsInterfaces)
                {
                    classSignature.Append(string.Join(COMMA + SPACE, ImplementsInterfaces));
                }
            }

            AddLine(sb, indent, classSignature.ToString());
        }

        private void AddFields(StringBuilder sb, int indent)
        {
            Fields.ForEach(field => AddLine(sb, indent, field.GenerateStringRepresentation()));
            if (Fields.Count > 0 && (Methods.Count > 0 || Properties.Count > 0 || Events.Count > 0)) AddEmptyLine(sb);
        }

        private void AddEvents(StringBuilder sb, int indent)
        {
            Events.ForEach(generatableEvent => AddLine(sb, indent, generatableEvent.GenerateStringRepresentation()));
            if (Events.Count > 0 && (Methods.Count > 0 || Properties.Count > 0)) AddEmptyLine(sb);
        }

        private void AddProperties(StringBuilder sb, int indent)
        {
            Properties.ForEach(property => AddLine(sb, indent, property.GenerateStringRepresentation()));
            if (Properties.Count > 0 && Methods.Count > 0) AddEmptyLine(sb);
        }

        private void AddMethods(StringBuilder sb, int indent)
        {
            int i = 0;
            foreach (GeneratableMethod method in Methods)
            {
                AddLines(sb, indent, method.GenerateStringRepresentationLines());
                if (i < Methods.Count - 1) AddEmptyLine(sb);
                i++;
            }
        }

        /// <summary>
        /// Add the member, replacing any existing one it would collide with. Replacing rather than
        /// silently skipping means a generator can add a default member and then override it, and that
        /// building the same type twice cannot quietly keep the stale version.
        /// </summary>
        private void Add<T>(T generatable, List<T> generatableList) where T : GeneratableBase
        {
            if (generatable == null)
            {
                return;
            }

            int existingIndex = generatableList.FindIndex(existing => generatable.DedupeKey == existing.DedupeKey);
            if (existingIndex >= 0)
            {
                generatableList[existingIndex] = generatable;
                return;
            }

            generatableList.Add(generatable);
        }
    }
}
