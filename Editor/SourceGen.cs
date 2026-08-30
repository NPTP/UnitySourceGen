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

        /// <summary>
        /// Write any generatable - class, struct or enum - to a path inside the project's Assets folder,
        /// e.g. "Assets/MyGame.Generated/PlayerActions.cs". Unlike the overloads that locate a file from an
        /// existing type, this can create files for types that do not exist yet, which is the normal case
        /// when generating a set of types that reference each other.
        /// <para>
        /// Returns whether the file was written, was already up to date, or failed. Nothing is logged on
        /// success, so a generator producing many files can report one summary of its own.
        /// </para>
        /// </summary>
        public static ScriptWriteResult WriteToPath(string pathInsideAssets, GeneratableBase generatable)
        {
            if (generatable == null)
            {
                Debug.LogError("Cannot write a null generatable.");
                return ScriptWriteResult.Failed;
            }

            return ScriptWriter.Write(new UnityAssetPath(pathInsideAssets), generatable.GenerateStringRepresentation());
        }

        /// <summary>
        /// Write raw file contents to a path inside the project's Assets folder, for anything the
        /// generatable types do not cover yet.
        /// </summary>
        public static ScriptWriteResult WriteToPath(string pathInsideAssets, string contents)
        {
            return ScriptWriter.Write(new UnityAssetPath(pathInsideAssets), contents);
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
