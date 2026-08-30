using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Extensions.Internal;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class GeneratableCodeChunkExtensions
    {
        public static GeneratableCodeChunk AddField<T>(this GeneratableCodeChunk gen, string fieldName, AccessModifier accessModifier) =>
            gen.AddField(TypeRef.From(typeof(T)), fieldName, accessModifier);

        public static GeneratableCodeChunk AddField(this GeneratableCodeChunk gen, TypeRef fieldType, string fieldName, AccessModifier accessModifier, string initialValueExpression = null)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField(fieldName, fieldType, accessModifier, isStatic: false, initialValueExpression));
            return gen;
        }

        public static GeneratableCodeChunk AddField<T>(this GeneratableCodeChunk gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue));
            return gen;
        }

        public static GeneratableCodeChunk AddStaticField<T>(this GeneratableCodeChunk gen, string fieldName, AccessModifier accessModifier) =>
            gen.AddStaticField(TypeRef.From(typeof(T)), fieldName, accessModifier);

        public static GeneratableCodeChunk AddStaticField(this GeneratableCodeChunk gen, TypeRef fieldType, string fieldName, AccessModifier accessModifier, string initialValueExpression = null)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField(fieldName, fieldType, accessModifier, isStatic: true, initialValueExpression));
            return gen;
        }

        public static GeneratableCodeChunk AddStaticField<T>(this GeneratableCodeChunk gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true, initialValue));
            return gen;
        }

        public static GeneratableCodeChunk AddConstField<T>(this GeneratableCodeChunk gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableConstField<T>(fieldName, accessModifier, initialValue));
            return gen;
        }

        public static GeneratableCodeChunk AddConstField(this GeneratableCodeChunk gen, TypeRef fieldType, string fieldName, AccessModifier accessModifier, string initialValueExpression)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableConstField(fieldName, fieldType, accessModifier, initialValueExpression));
            return gen;
        }

        public static GeneratableCodeChunk AddGetterProperty<T>(this GeneratableCodeChunk gen, string propertyName, string fieldName, AccessModifier getModifier, bool isStatic, CustomSyntax getterSyntax) =>
            gen.AddGetterProperty(TypeRef.From(typeof(T)), propertyName, fieldName, getModifier, isStatic, getterSyntax);

        public static GeneratableCodeChunk AddGetterProperty(this GeneratableCodeChunk gen, TypeRef propertyType, string propertyName, string fieldName, AccessModifier getModifier, bool isStatic, CustomSyntax getterSyntax)
        {
            if (!propertyName.CheckValidGenerationName() || !fieldName.CheckValidGenerationName()) return gen;
            gen.AddProperty(new GeneratableGetterProperty(propertyName, propertyType, fieldName, getModifier, isStatic, getterSyntax));
            return gen;
        }

        public static GeneratableCodeChunk AddStaticMethod<T>(this GeneratableCodeChunk gen, string methodName, AccessModifier accessModifier, params string[] body) =>
            gen.AddStaticMethod(TypeRef.From(typeof(T)), methodName, accessModifier, body);

        public static GeneratableCodeChunk AddStaticMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, params string[] body) =>
            gen.AddStaticMethod(returnType, methodName, accessModifier, GeneratableParameter.None, body);

        public static GeneratableCodeChunk AddStaticMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableParameter[] parameters, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: true, parameters, isExpressionBodied: false, body));
            return gen;
        }

        public static GeneratableCodeChunk AddMethod<T>(this GeneratableCodeChunk gen, string methodName, AccessModifier accessModifier, params string[] body) =>
            gen.AddMethod(TypeRef.From(typeof(T)), methodName, accessModifier, InheritanceModifier.None, body);

        public static GeneratableCodeChunk AddMethod<T>(this GeneratableCodeChunk gen, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body) =>
            gen.AddMethod(TypeRef.From(typeof(T)), methodName, accessModifier, inheritanceModifier, body);

        public static GeneratableCodeChunk AddMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body) =>
            gen.AddMethod(returnType, methodName, accessModifier, inheritanceModifier, GeneratableParameter.None, body);

        public static GeneratableCodeChunk AddMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, GeneratableParameter[] parameters, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, inheritanceModifier, isStatic: false, parameters, isExpressionBodied: false, body));
            return gen;
        }

        public static GeneratableCodeChunk AddEmptyLine(this GeneratableCodeChunk gen)
        {
            gen.AddEmptyLine();
            return gen;
        }

        public static GeneratableCodeChunk AddComment(this GeneratableCodeChunk gen, string comment)
        {
            if (!comment.CheckValidGenerationName()) return gen;
            gen.AddComment(new GeneratableComment(comment));
            return gen;
        }

        #region Unity Centric

        public static GeneratableCodeChunk AddSerializedField<T>(this GeneratableCodeChunk gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            var field = new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue);
            field.AddAttribute(new SerializeFieldAttribute());
            gen.AddField(field);
            return gen;
        }

        public static GeneratableCodeChunk AddSerializedProperty<T>(this GeneratableCodeChunk gen, string fieldName, bool isStatic, CustomSyntax? getterValueSyntax = null)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;

            var field = new GeneratableField<T>(fieldName, AccessModifier.Private, isStatic: false);
            field.AddAttribute(new SerializeFieldAttribute());
            gen.AddField(field);

            gen.AddGetterProperty<T>(fieldName.UppercaseFirst(), fieldName, AccessModifier.Public, isStatic, getterValueSyntax ?? CustomSyntax.Default);

            return gen;
        }

        #endregion

        #region Expression Bodied

        /// <summary>Writes "returnType Name(parameters) =&gt; expression;".</summary>
        public static GeneratableCodeChunk AddExpressionBodiedMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableParameter[] parameters, string expression)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: false, parameters, isExpressionBodied: true, expression));
            return gen;
        }

        public static GeneratableCodeChunk AddStaticExpressionBodiedMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableParameter[] parameters, string expression)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: true, parameters, isExpressionBodied: true, expression));
            return gen;
        }

        #endregion

        #region Generic Methods

        public static GeneratableCodeChunk AddGenericMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableTypeParameter[] typeParameters, GeneratableParameter[] parameters, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: false, typeParameters, parameters, isExpressionBodied: false, body));
            return gen;
        }

        public static GeneratableCodeChunk AddStaticGenericMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableTypeParameter[] typeParameters, GeneratableParameter[] parameters, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: true, typeParameters, parameters, isExpressionBodied: false, body));
            return gen;
        }

        public static GeneratableCodeChunk AddExpressionBodiedGenericMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableTypeParameter[] typeParameters, GeneratableParameter[] parameters, string expression)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: false, typeParameters, parameters, isExpressionBodied: true, expression));
            return gen;
        }

        public static GeneratableCodeChunk AddStaticExpressionBodiedGenericMethod(this GeneratableCodeChunk gen, TypeRef returnType, string methodName, AccessModifier accessModifier, GeneratableTypeParameter[] typeParameters, GeneratableParameter[] parameters, string expression)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod(methodName, returnType, accessModifier, InheritanceModifier.None, isStatic: true, typeParameters, parameters, isExpressionBodied: true, expression));
            return gen;
        }

        #endregion
    }
}
