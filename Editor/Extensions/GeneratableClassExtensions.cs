using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Options;

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

        public static GeneratableTypeDefinition WithField<T>(this GeneratableTypeDefinition gen, NameSyntax fieldName, AccessModifier accessModifier)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false));
            return gen;
        }

        public static GeneratableTypeDefinition WithField<T>(this GeneratableTypeDefinition gen, NameSyntax fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithStaticField<T>(this GeneratableTypeDefinition gen, NameSyntax fieldName, AccessModifier accessModifier)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticField<T>(this GeneratableTypeDefinition gen, NameSyntax fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true, initialValue));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithConstField<T>(this GeneratableTypeDefinition gen, NameSyntax fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableConstField<T>(fieldName, accessModifier, initialValue));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticMethod<T>(this GeneratableTypeDefinition gen, NameSyntax methodName, AccessModifier accessModifier, params string[] body)
        {
            if (!ExtensionsCommon.CheckValidName(methodName)) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: true, body));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, NameSyntax methodName, AccessModifier accessModifier, params string[] body)
        {
            if (!ExtensionsCommon.CheckValidName(methodName)) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: false, body));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, NameSyntax methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body)
        {
            if (!ExtensionsCommon.CheckValidName(methodName)) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, inheritanceModifier, isStatic: false, body));
            return gen;
        }
    }
}
