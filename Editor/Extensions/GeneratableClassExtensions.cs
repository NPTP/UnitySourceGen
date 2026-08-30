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

        public static GeneratableTypeDefinition WithStaticMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, params string[] body) =>
            gen.WithStaticMethod(returnType, methodName, accessModifier, GeneratableParameter.None, body);

        public static GeneratableTypeDefinition WithStaticMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableParameter[] parameters, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: true, parameters, isExpressionBodied: false, body));
            return gen;
        }

        /// <summary>
        /// An extension method: a static method whose first parameter carries the "this" modifier. The
        /// containing class must be static.
        /// </summary>
        public static GeneratableTypeDefinition WithExtensionMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableParameter extendedParameter, GeneratableParameter[] additionalParameters, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: true, Prepend(extendedParameter, additionalParameters), isExpressionBodied: false, body));
            return gen;
        }

        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, params string[] body) =>
            gen.WithMethod(TypeRef.From(typeof(T)), methodName, accessModifier, InheritanceModifier.None, body);

        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body) =>
            gen.WithMethod(TypeRef.From(typeof(T)), methodName, accessModifier, inheritanceModifier, body);

        public static GeneratableTypeDefinition WithMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body) =>
            gen.WithMethod(returnType, methodName, accessModifier, inheritanceModifier, GeneratableParameter.None, body);

        public static GeneratableTypeDefinition WithMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, GeneratableParameter[] parameters, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, inheritanceModifier, isStatic: false, parameters, isExpressionBodied: false, body));
            return gen;
        }

        #region Expression Bodied

        /// <summary>Writes "returnType Name(parameters) =&gt; expression;".</summary>
        public static GeneratableTypeDefinition WithExpressionBodiedMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableParameter[] parameters, string expression)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: false, parameters, isExpressionBodied: true, expression));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticExpressionBodiedMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableParameter[] parameters, string expression)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: true, parameters, isExpressionBodied: true, expression));
            return gen;
        }

        /// <summary>An expression-bodied extension method. The containing class must be static.</summary>
        public static GeneratableTypeDefinition WithExpressionBodiedExtensionMethod(this GeneratableTypeDefinition gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableParameter extendedParameter, GeneratableParameter[] additionalParameters, string expression)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: true, Prepend(extendedParameter, additionalParameters), isExpressionBodied: true, expression));
            return gen;
        }

        internal static GeneratableParameter[] Prepend(GeneratableParameter first, GeneratableParameter[] rest)
        {
            GeneratableParameter[] all = new GeneratableParameter[1 + (rest?.Length ?? 0)];
            all[0] = first;
            rest?.CopyTo(all, 1);
            return all;
        }

        #endregion
    }
}
