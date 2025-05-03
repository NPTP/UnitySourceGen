using System;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Extensions;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.UnitySourceGen.Editor.ScriptWriting;

namespace NPTP.UnitySourceGen.Editor
{
    public static class SourceGen
    {
        public static GeneratableClass NewClass(string name, AccessModifier accessModifier) => new GeneratableClass(name, accessModifier, isStatic: false);
        public static GeneratableClass NewStaticClass(string name, AccessModifier accessModifier) => new GeneratableClass(name, accessModifier, isStatic: true);
        public static GeneratableEnum NewEnum(string name, AccessModifier accessModifier) => new GeneratableEnum(name, accessModifier);
        
        public static bool ReplaceClassInAssetsScriptFile(Type classType, GeneratableClass generatableClass)
        {
            generatableClass.InNamespace(classType.Namespace);
            return ScriptWriter.TryReplaceClass(classType, generatableClass);
        }
    }
}
