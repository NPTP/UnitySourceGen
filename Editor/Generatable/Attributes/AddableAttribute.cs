
namespace NPTP.UnitySourceGen.Editor.Generatable.Attributes
{
    public class AddableAttribute
    {
        public string Name { get; }
        
        public AddableAttribute(string attributeName)
        {
            Name = attributeName;
        }

        public string GetStringRepresentation() => $"[{Name}]";

        public override bool Equals(object obj)
        {
            return obj is AddableAttribute other && other == this;
        }

        protected bool Equals(AddableAttribute other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}
