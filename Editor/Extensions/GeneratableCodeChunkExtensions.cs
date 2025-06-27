using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using NPTP.UnitySourceGen.Editor.Options;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class GeneratableCodeChunkExtensions
    {
        public static GeneratableCodeChunk AddField<T>(this GeneratableCodeChunk gen, NameSyntax fieldName, AccessModifier accessModifier)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false));
            return gen;
        }

        public static GeneratableCodeChunk AddField<T>(this GeneratableCodeChunk gen, NameSyntax fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue));
            return gen;
        }
        
        public static GeneratableCodeChunk AddStaticField<T>(this GeneratableCodeChunk gen, NameSyntax fieldName, AccessModifier accessModifier)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true));
            return gen;
        }

        public static GeneratableCodeChunk AddStaticField<T>(this GeneratableCodeChunk gen, NameSyntax fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true, initialValue));
            return gen;
        }
        
        public static GeneratableCodeChunk AddConstField<T>(this GeneratableCodeChunk gen, NameSyntax fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableConstField<T>(fieldName, accessModifier, initialValue));
            return gen;
        }

        public static GeneratableCodeChunk AddGetterProperty<T>(this GeneratableCodeChunk gen, NameSyntax propertyName, NameSyntax getterSyntax, AccessModifier getModifier, bool isStatic)
        {
            if (!ExtensionsCommon.CheckValidName(propertyName)) return gen;
            gen.AddProperty(new GeneratableGetterProperty<T>(propertyName, getterSyntax, getModifier, isStatic));
            return gen;
        }
        
        public static GeneratableCodeChunk AddStaticMethod<T>(this GeneratableCodeChunk gen, NameSyntax methodName, AccessModifier accessModifier, params string[] body)
        {
            if (!ExtensionsCommon.CheckValidName(methodName)) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: true, body));
            return gen;
        }
        
        public static GeneratableCodeChunk AddMethod<T>(this GeneratableCodeChunk gen, NameSyntax methodName, AccessModifier accessModifier, params string[] body)
        {
            if (!ExtensionsCommon.CheckValidName(methodName)) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: false, body));
            return gen;
        }
        
        public static GeneratableCodeChunk AddMethod<T>(this GeneratableCodeChunk gen, NameSyntax methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body)
        {
            if (!ExtensionsCommon.CheckValidName(methodName)) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, inheritanceModifier, isStatic: false, body));
            return gen;
        }
        
        #region Unity Centric

        public static GeneratableCodeChunk AddSerializedField<T>(this GeneratableCodeChunk gen, NameSyntax fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;
            var field = new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue);
            field.AddAttribute(new SerializeFieldAttribute());
            gen.AddField(field);
            return gen;
        }

        public static GeneratableCodeChunk AddSerializedProperty<T>(this GeneratableCodeChunk gen, NameSyntax fieldName, NameSyntax propertyName, NameSyntax getterSyntax, bool getterIsStatic)
        {
            if (!ExtensionsCommon.CheckValidName(fieldName)) return gen;

            var field = new GeneratableField<T>(fieldName.WithLowerCaseName(), AccessModifier.Private, isStatic: false);
            field.AddAttribute(new SerializeFieldAttribute());
            gen.AddField(field);

            gen.AddGetterProperty<T>(propertyName.WithUpperCaseName(), getterSyntax, AccessModifier.Public, getterIsStatic);
            
            return gen;
        }

        #endregion
    }
}
