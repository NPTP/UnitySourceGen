using System.Collections.Generic;
using System.Text;
using NPTP.UnitySourceGen.Editor.Enums;

namespace NPTP.UnitySourceGen.Editor.Generatable
{
    public class GeneratableCodeChunk : GeneratableBase
    {
        private List<GeneratableBase> Members { get; } = new();
        
        internal int Indent { get; set; }
        
        internal GeneratableCodeChunk(string name, AccessModifier accessModifier, bool isStatic) : base(name, accessModifier, isStatic)
        {
        }

        public override string GenerateStringRepresentation()
        {
            StringBuilder sb = new();

            foreach (GeneratableBase member in Members)
            {
                switch (member)
                {
                    case GeneratableField field:
                        AddLine(sb, Indent, field.GenerateStringRepresentation());
                        break;
                    case GeneratableProperty property:
                        AddLine(sb, Indent, property.GenerateStringRepresentation());
                        break;
                    case GeneratableMethod method:
                        AddLines(sb, Indent, method.GenerateStringRepresentationLines());
                        break;
                    case GeneratableEmptyMember emptyMember:
                        AddLine(sb, Indent, emptyMember.GenerateStringRepresentation());
                        break;
                }
            }
            
            return sb.ToString();
        }
        
        internal void AddEmptyLine()
        {
            Members.Add(new GeneratableEmptyMember());
        }
        
        internal void AddComment(GeneratableComment comment)
        {
            Members.Add(comment);
        }

        internal void AddField(GeneratableField field) => Members.Add(field);
        internal void AddProperty(GeneratableProperty property) => Members.Add(property);
        internal void AddMethod(GeneratableMethod method) => Members.Add(method);
    }
}
