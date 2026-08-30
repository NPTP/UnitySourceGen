using System.Collections.Generic;
using System.Text;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    /// <summary>
    /// A loose run of members with no type around them, for dropping into a region of an existing script
    /// through <see cref="Modifiable.ModifiableScript"/>. Members are written in the order added, and the
    /// chunk is indented to match the code already in the region.
    /// <code>
    /// SourceGen.NewCodeChunk()
    ///     .WithComment("Generated - do not edit")
    ///     .WithField(SourceGen.NewField&lt;int&gt;("count").Private())
    ///     .WithEmptyLine()
    ///     .WithMethod(SourceGen.NewMethod("Reset").Public().ReturningVoid().Body("count = 0;"));
    /// </code>
    /// </summary>
    public class GeneratableCodeChunk : GeneratableBase
    {
        private List<GeneratableBase> Members { get; } = new();

        internal int Indent { get; set; }

        internal GeneratableCodeChunk() : base(string.Empty, sanitizeName: false) { }

        public GeneratableCodeChunk WithField(GeneratableField field)
        {
            if (field != null) Members.Add(field);
            return this;
        }

        public GeneratableCodeChunk WithEvent(GeneratableEvent generatableEvent)
        {
            if (generatableEvent != null) Members.Add(generatableEvent);
            return this;
        }

        public GeneratableCodeChunk WithProperty(GeneratableProperty property)
        {
            if (property != null) Members.Add(property);
            return this;
        }

        public GeneratableCodeChunk WithMethod(GeneratableMethod method)
        {
            if (method != null) Members.Add(method);
            return this;
        }

        public GeneratableCodeChunk WithComment(string comment)
        {
            if (!string.IsNullOrEmpty(comment)) Members.Add(new GeneratableComment(comment));
            return this;
        }

        public GeneratableCodeChunk WithEmptyLine()
        {
            Members.Add(new GeneratableEmptyMember());
            return this;
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder sb = new();

            foreach (GeneratableBase member in Members)
            {
                switch (member)
                {
                    case GeneratableMethod method:
                        AddLines(sb, Indent, method.GenerateStringRepresentationLines());
                        break;
                    default:
                        AddLine(sb, Indent, member.GenerateStringRepresentation());
                        break;
                }
            }

            return sb.ToString();
        }
    }
}
