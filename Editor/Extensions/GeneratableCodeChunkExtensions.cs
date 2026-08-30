using System;
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

        /// <summary>
        /// Add a method. See the documentation on <see cref="GeneratableMethodBuilder"/> for what can be
        /// configured.
        /// </summary>
        public static GeneratableCodeChunk AddMethod(this GeneratableCodeChunk gen, string methodName, Action<GeneratableMethodBuilder> configure)
        {
            if (!methodName.CheckValidGenerationName()) return gen;

            GeneratableMethodBuilder builder = new(methodName);
            configure?.Invoke(builder);
            gen.AddMethod(builder.Build());
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

    }
}
