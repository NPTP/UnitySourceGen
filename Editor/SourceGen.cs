using System;
using System.IO;
using System.Linq;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Extensions;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Modifiable;
using NPTP.UnitySourceGen.Editor.ScriptWriting;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor
{
    public static class SourceGen
    {
        public static GeneratableClass NewClass(string name, AccessModifier accessModifier) => new GeneratableClass(name, accessModifier, isStatic: false);
        public static GeneratableClass NewStaticClass(string name, AccessModifier accessModifier) => new GeneratableClass(name, accessModifier, isStatic: true);
        public static GeneratableEnum NewEnum(string name, AccessModifier accessModifier) => new GeneratableEnum(name, accessModifier);
        public static GeneratableCodeChunk NewCodeChunk() => new GeneratableCodeChunk(default, default, default);
        
        public static ModifiableScript GetScriptToModify<T>()
        {
            if (AssetsScriptGetter.TryGetSystemFilePathToScriptInAssets(typeof(T), out UnityAssetPath unityAssetPath))
            {
                try
                { 
                    return new ModifiableScript(File.ReadAllLines(unityAssetPath.SystemPath).ToList(), unityAssetPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not read script: {e.Message}");
                }
            }

            return null;
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
