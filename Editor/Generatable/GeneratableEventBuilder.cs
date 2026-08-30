using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// Configures one generated event.
    /// <code>
    /// // public static event Action OnControlsUpdated;
    /// gen.WithEvent("OnControlsUpdated", e =&gt; e.Public().Static().Of("Action"));
    ///
    /// // public static event Action OnControlsUpdated { add =&gt; Runtime.OnControlsUpdated += value; ... }
    /// gen.WithEvent("OnControlsUpdated", e =&gt; e.Public().Static().Of("Action").Forwarding("Runtime.OnControlsUpdated"));
    /// </code>
    /// </summary>
    public class GeneratableEventBuilder
    {
        private readonly string name;

        private TypeRef handlerType = new("Action");
        private AccessModifier accessModifier = AccessModifier.Private;
        private bool isStatic;
        private string addExpression;
        private string removeExpression;

        internal GeneratableEventBuilder(string name)
        {
            this.name = name;
        }

        /// <summary>The delegate type of the event, e.g. "Action" or "Action&lt;InputPlayer&gt;".</summary>
        public GeneratableEventBuilder Of(TypeRef delegateType)
        {
            handlerType = delegateType;
            return this;
        }

        public GeneratableEventBuilder Of<T>() => Of(TypeRef.From(typeof(T)));

        public GeneratableEventBuilder WithAccess(AccessModifier modifier)
        {
            accessModifier = modifier;
            return this;
        }

        public GeneratableEventBuilder Public() => WithAccess(AccessModifier.Public);
        public GeneratableEventBuilder Private() => WithAccess(AccessModifier.Private);
        public GeneratableEventBuilder Protected() => WithAccess(AccessModifier.Protected);
        public GeneratableEventBuilder Internal() => WithAccess(AccessModifier.Internal);

        public GeneratableEventBuilder Static()
        {
            isStatic = true;
            return this;
        }

        /// <summary>
        /// Subscribe and unsubscribe on another event, which is how a facade re-exposes the event of the
        /// object it wraps. Writes "add =&gt; target += value; remove =&gt; target -= value;".
        /// </summary>
        public GeneratableEventBuilder Forwarding(string targetEventExpression)
        {
            addExpression = targetEventExpression + " += value";
            removeExpression = targetEventExpression + " -= value";
            return this;
        }

        /// <summary>Accessor bodies written verbatim, for anything <see cref="Forwarding"/> does not cover.</summary>
        public GeneratableEventBuilder WithAccessors(string addBodyExpression, string removeBodyExpression)
        {
            addExpression = addBodyExpression;
            removeExpression = removeBodyExpression;
            return this;
        }

        internal GeneratableEvent Build() => new(name, handlerType, accessModifier, isStatic, addExpression, removeExpression);
    }
}
