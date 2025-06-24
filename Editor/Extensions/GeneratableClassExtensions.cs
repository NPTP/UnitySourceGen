using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Generatable.Attributes;
using UnityEngine;

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
            if (!CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false));
            return gen;
        }

        public static GeneratableTypeDefinition WithField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithStaticField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier)
        {
            if (!CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableField<T>(fieldName, accessModifier, isStatic: true, initialValue));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithConstField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!CheckValidName(fieldName)) return gen;
            gen.AddField(new GeneratableConstField<T>(fieldName, accessModifier, initialValue));
            return gen;
        }

        public static GeneratableTypeDefinition WithGetterProperty<T>(this GeneratableTypeDefinition gen, string propertyName, AccessModifier getModifier, GeneratableField fieldToGet, bool isStatic)
        {
            if (!CheckValidName(propertyName)) return gen;
            gen.AddProperty(new GeneratableGetterProperty<T>(propertyName, getModifier, isStatic, fieldToGet));
            return gen;
        }

        public static GeneratableTypeDefinition WithStaticMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, params string[] body)
        {
            if (!CheckValidName(methodName)) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: true, body));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, params string[] body)
        {
            if (!CheckValidName(methodName)) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, InheritanceModifier.None, isStatic: false, body));
            return gen;
        }
        
        public static GeneratableTypeDefinition WithMethod<T>(this GeneratableTypeDefinition gen, string methodName, AccessModifier accessModifier, InheritanceModifier inheritanceModifier, params string[] body)
        {
            if (!CheckValidName(methodName)) return gen;
            gen.AddMethod(new GeneratableMethod<T>(methodName, accessModifier, inheritanceModifier, isStatic: false, body));
            return gen;
        }

        #region Unity Centric

        public static GeneratableTypeDefinition WithSerializedField<T>(this GeneratableTypeDefinition gen, string fieldName, AccessModifier accessModifier, T initialValue)
        {
            if (!CheckValidName(fieldName)) return gen;
            var field = new GeneratableField<T>(fieldName, accessModifier, isStatic: false, initialValue);
            field.AddAttribute(new SerializeFieldAttribute());
            gen.AddField(field);
            return gen;
        }

        /// <summary>
        /// Generate a serialized private backing field with a public accessor property. For example:
        /// WithSerializedProperty<int>("myProperty", 6) will produce:
        ///     [SerializeField] private int myProperty = 6;
        ///     public int MyProperty => myProperty;
        /// </summary>
        public static GeneratableTypeDefinition WithSerializedProperty<T>(this GeneratableTypeDefinition gen, string fieldName, T initialValue = default)
        {
            if (!CheckValidName(fieldName)) return gen;

            if (gen.IsStatic)
            {
                Debug.LogWarning("Can't implement a serialized field on a static object.");
                return gen;
            }
            
            var field = new GeneratableField<T>(fieldName, AccessModifier.Private, isStatic: false, initialValue);
            field.AddAttribute(new SerializeFieldAttribute());
            gen.AddField(field);

            gen.WithGetterProperty<T>(fieldName.UppercaseFirst(), AccessModifier.Public, field, isStatic: false);
            
            return gen;
        }

        #endregion

        private static bool CheckValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("Tried to add a null- or empty-named element to a generatable type definition.");
                return false;
            }
            
            return true;
        }
        
    }
}
