using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Extensions.Internal;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Syntax;

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

        public static GeneratableTypeDefinition InheritsFrom<T>(this GeneratableTypeDefinition gen) => gen.InheritsFrom(TypeRef.From(typeof(T)));

        public static GeneratableTypeDefinition InheritsFrom(this GeneratableTypeDefinition gen, TypeRef baseType)
        {
            gen.BaseClassTypeName = baseType.Name;
            return gen;
        }

        public static GeneratableTypeDefinition ImplementsInterface<T>(this GeneratableTypeDefinition gen) where T : class => gen.ImplementsInterface(TypeRef.From(typeof(T)));

        public static GeneratableTypeDefinition ImplementsInterface(this GeneratableTypeDefinition gen, TypeRef interfaceType)
        {
            gen.ImplementsInterfaces.Add(interfaceType.Name);
            return gen;
        }

        public static GeneratableTypeDefinition WithField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier) =>
            gen.WithField(TypeRef.From(typeof(T)), fieldName, accessModifier);

        public static GeneratableTypeDefinition WithField(this GeneratableTypeDefinition gen, TypeRef fieldType, string fieldName, AccessModifier accessModifier, string initialValueExpression = null)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField(fieldName, fieldType, accessModifier, isStatic: false, initialValueExpression));
            return gen;
        }

        public static GeneratableTypeDefinition WithField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier) =>
            gen.WithStaticField(TypeRef.From(typeof(T)), fieldName, accessModifier);

        public static GeneratableTypeDefinition WithStaticField(this GeneratableTypeDefinition gen, TypeRef fieldType, string fieldName, AccessModifier accessModifier, string initialValueExpression = null)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField(fieldName, fieldType, accessModifier, isStatic: true, initialValueExpression));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true, initialValue));
            return gen;
        }

        public static GeneratableTypeDefinition WithConstField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableConstField<T>(fieldName, accessModifier, initialValue));
            return gen;
        }

        public static GeneratableTypeDefinition WithConstField(this GeneratableTypeDefinition gen, TypeRef fieldType, string fieldName, AccessModifier accessModifier, string initialValueExpression)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableConstField(fieldName, fieldType, accessModifier, initialValueExpression));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, params string[] body) =>
            gen.WithStaticMethod(TypeRef.From(typeof(T)), methodName, accessModifier, body);

        public static GeneratableTypeDefinition WithStaticMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: true, body));
            return gen;
        }

        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, params string[] body) =>
            gen.WithMethod(TypeRef.From(typeof(T)), methodName, accessModifier, InheritanceModifier.None, body);

        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body) =>
            gen.WithMethod(TypeRef.From(typeof(T)), methodName, accessModifier, inheritanceModifier, body);

        public static GeneratableTypeDefinition WithMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, inheritanceModifier, isStatic: false, body));
            return gen;
        }
    }
}
