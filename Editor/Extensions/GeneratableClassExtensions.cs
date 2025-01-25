using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class GeneratableClassExtensions
    {
      public static GeneratableTypeDefinition WithInheritanceModifier(this GeneratableTypeDefinition gen, InheritanceModifier inheritanceModifier)
        {
            gen.InheritanceModifier = inheritanceModifier;
            return gen;
        }

        public static GeneratableTypeDefinition AsPartial(this GeneratableTypeDefinition gen)
        {
            gen.IsPartial = true;
            return gen;
        }
        
        public static GeneratableTypeDefinition InheritsFrom<T>(this GeneratableTypeDefinition gen)
        {
            gen.BaseClassTypeName = typeof(T).Name;
            return gen;
        }
        
        public static GeneratableTypeDefinition ImplementsInterface<T>(this GeneratableTypeDefinition gen) where T : class
        {
            gen.ImplementsInterfaces.Add(typeof(T).Name);
            return gen;
        }

        public static GeneratableTypeDefinition WithField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier)
        {
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false));
            return gen;
        }

        public static GeneratableTypeDefinition WithField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithStaticField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier)
        {
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true, initialValue));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithConstField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            gen.AddField(new GeneratableConstField<T>(fieldName, accessModifier, initialValue));
            return gen;
        }

        public static GeneratableTypeDefinition WithProperty(this GeneratableTypeDefinition gen, string propertyName, AccessModifier getModifier, AccessModifier setModifier, bool isStatic)
        {
            gen.AddProperty(new GeneratableProperty(propertyName, getModifier, setModifier, isStatic));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, params string[] body)
        {
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: true, body));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, params string[] body)
        {
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: false, body));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body)
        {
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, inheritanceModifier, isStatic: false, body));
            return gen;
        }
        
    }
}
