using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// An event, either field-like ("public event Action OnFoo;") or with accessors
    /// ("public event Action OnFoo { add =&gt; target += value; remove =&gt; target -= value; }").
    /// </summary>
    public class GeneratableEvent : GeneratableBase
    {
        private const string EVENT = "event";

        private readonly TypeRef handlerType;
        private readonly string addExpression;
        private readonly string removeExpression;

        private bool HasAccessors => addExpression != null || removeExpression != null;

        internal GeneratableEvent(string name, TypeRef handlerType, AccessModifier accessModifier, bool isStatic, string addExpression, string removeExpression)
            : base(name, accessModifier, isStatic)
        {
            this.handlerType = handlerType;
            this.addExpression = addExpression;
            this.removeExpression = removeExpression;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder sb = new();

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
