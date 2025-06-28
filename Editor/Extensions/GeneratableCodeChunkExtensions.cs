using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Syntax;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class GeneratableCodeChunkExtensions
    {
        public static GeneratableCodeChunk AddField<T>(this GeneratableCodeChunk gen, string fieldName, AccessModifier accessModifier)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false));
            return gen;
        }

        public static GeneratableCodeChunk AddField<T>(this GeneratableCodeChunk gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue));
            return gen;
        }
        
        public static GeneratableCodeChunk AddStaticField<T>(this GeneratableCodeChunk gen, string fieldName, AccessModifier accessModifier)
        {
            if (!fieldName.CheckValidGenerationName()) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true));
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

        public static GeneratableCodeChunk AddGetterProperty<T>(this GeneratableCodeChunk gen, string propertyName, string fieldName, AccessModifier getModifier, bool isStatic, CustomSyntax getterSyntax)
        {
            if (!propertyName.CheckValidGenerationName() || !fieldName.CheckValidGenerationName()) return gen;
            gen.AddProperty(new GeneratableGetterProperty<T>(propertyName, fieldName, getModifier, isStatic, getterSyntax));
            return gen;
        }
        
        public static GeneratableCodeChunk AddStaticMethod<T>(this GeneratableCodeChunk gen, string methodName, AccessModifier accessModifier, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: true, body));
            return gen;
        }
        
        public static GeneratableCodeChunk AddMethod<T>(this GeneratableCodeChunk gen, string methodName, AccessModifier accessModifier, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: false, body));
            return gen;
        }
        
        public static GeneratableCodeChunk AddMethod<T>(this GeneratableCodeChunk gen, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body)
        {
            if (!methodName.CheckValidGenerationName()) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, inheritanceModifier, isStatic: false, body));
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
