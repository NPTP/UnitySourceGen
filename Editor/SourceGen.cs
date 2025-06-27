using System;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Extensions;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Modifiable;
using NPTP.UnitySourceGen.Editor.ScriptWriting;

namespace NPTP.UnitySourceGen.Editor
{
    public static class SourceGen
    {
        public static GeneratableClass NewClass(string name, AccessModifier accessModifier) => new GeneratableClass(name, accessModifier, isStatic: false);
        public static GeneratableClass NewStaticClass(string name, AccessModifier accessModifier) => new GeneratableClass(name, accessModifier, isStatic: true);
        public static GeneratableEnum NewEnum(string name, AccessModifier accessModifier) => new GeneratableEnum(name, accessModifier);
        public static GeneratableCodeChunk NewCodeChunk() => new GeneratableCodeChunk(default, default, default);


        public static ModifiableScript<T> GetScriptToModify<T>()
        {
            return new ModifiableScript<T>();
        }

        public static bool WriteCodeChunkInScript<T>(GeneratableCodeChunk codeChunk, string sectionStartMarker, string sectionEndMarker)
        {
            if (!AssetsScriptGetter.TryGetSystemFilePathToScriptInAssets<T>(out UnityAssetPath unityAssetPath))
            {
                return false;
            }

            return ScriptWriter.TryReplaceSection(unityAssetPath, new[] { sectionStartMarker }, sectionEndMarker, codeChunk);
        }

        public static bool WriteCodeChunkInScriptRegion<T>(GeneratableCodeChunk codeChunk, string regionName)
        {
            if (!AssetsScriptGetter.TryGetSystemFilePathToScriptInAssets<T>(out UnityAssetPath unityAssetPath))
            {
                return false;
            }

            return ScriptWriter.TryReplaceSection(unityAssetPath, new[] { "#region", regionName }, "#endregion", codeChunk);
        }
        
        public static bool WriteClassToAssetsScriptFile<T>(string pathInsideAssets, GeneratableClass generatableClass)
        {
            if (!AssetsScriptGetter.TryGetSystemFilePathToScriptInAssets<T>(out UnityAssetPath unityAssetPath))
            {
                return false;
            }
            
            return ScriptWriter.TryWrite(unityAssetPath, generatableClass.GenerateStringRepresentation());
        }
        
        public static bool ReplaceClassInAssetsScriptFile(Type classType, GeneratableClass generatableClass)
        {
            generatableClass.InNamespace(classType.Namespace);
            return ScriptWriter.TryReplaceClass(classType, generatableClass);
        }
    }
}
