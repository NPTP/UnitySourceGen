using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// An event, either field-like ("public event Action OnFoo;") or with accessors
    /// ("public event Action OnFoo { add =&gt; target += value; remove =&gt; target -= value; }").
    /// <code>
    /// SourceGen.NewEvent("OnControlsUpdated").Public().Static().Of("Action")
    /// SourceGen.NewEvent("OnControlsUpdated").Public().Static().Of("Action").Forwarding("Runtime.OnControlsUpdated")
    /// </code>
    /// </summary>
    public class GeneratableEvent : GeneratableBase
    {
        private const string EVENT = "event";

        private TypeRef handlerType = new("Action");
        private string addExpression;
        private string removeExpression;

        private bool HasAccessors => addExpression != null || removeExpression != null;

        internal GeneratableEvent(string name) : base(name, AccessModifier.Private, isStatic: false) { }

        /// <summary>The delegate type of the event, e.g. "Action" or "Action&lt;InputPlayer&gt;".</summary>
        public GeneratableEvent Of(TypeRef delegateType)
        {
            handlerType = delegateType;
            return this;
        }

        public GeneratableEvent Of<T>() => Of(TypeRef.From(typeof(T)));

        public GeneratableEvent WithAccess(AccessModifier modifier)
        {
            AccessModifier = modifier;
            return this;
        }

        public GeneratableEvent Public() => WithAccess(AccessModifier.Public);
        public GeneratableEvent Private() => WithAccess(AccessModifier.Private);
        public GeneratableEvent Protected() => WithAccess(AccessModifier.Protected);
        public GeneratableEvent Internal() => WithAccess(AccessModifier.Internal);

        public GeneratableEvent Static()
        {
            IsStatic = true;
            return this;
        }

        public GeneratableEvent WithAttribute(AddableAttribute attribute)
        {
            AddAttribute(attribute);
            return this;
        }

        public GeneratableEvent WithAttribute(string attributeName, params string[] arguments) =>
            WithAttribute(new AddableAttribute(attributeName, arguments));

        public GeneratableEvent OnlyIf(string conditionalCompilationSymbol)
        {
            ConditionalCompilationSymbol = conditionalCompilationSymbol;
            return this;
        }

        /// <summary>
        /// Subscribe and unsubscribe on another event, which is how a facade re-exposes the event of the
        /// object it wraps. Writes "add =&gt; target += value; remove =&gt; target -= value;".
        /// </summary>
        public GeneratableEvent Forwarding(string targetEventExpression)
        {
            addExpression = targetEventExpression + " += value";
            removeExpression = targetEventExpression + " -= value";
            return this;
        }

        /// <summary>Accessor bodies written verbatim, for anything <see cref="Forwarding"/> does not cover.</summary>
        public GeneratableEvent WithAccessors(string addBodyExpression, string removeBodyExpression)
        {
            addExpression = addBodyExpression;
            removeExpression = removeBodyExpression;
            return this;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder sb = new();

            sb.Append(GetAttributesInline());
            sb.Append(AccessModifier.AsString());
            if (IsStatic) sb.Append(SPACE + STATIC);
            sb.Append(SPACE + EVENT);
            sb.Append(SPACE + handlerType.Name);
            sb.Append(SPACE + Name);

            if (!HasAccessors)
            {
                sb.Append(SEMICOLON);
                return sb.ToString();
            }

            sb.Append(SPACE + "{");
            if (addExpression != null) sb.Append(SPACE + "add => " + addExpression + SEMICOLON);
            if (removeExpression != null) sb.Append(SPACE + "remove => " + removeExpression + SEMICOLON);
            sb.Append(SPACE + "}");

            return sb.ToString();
        }
    }
}
