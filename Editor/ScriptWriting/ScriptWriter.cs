using System;
using System.IO;
using NPTP.UnitySourceGen.Editor.Generatable;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.ScriptWriting
{
    internal static class ScriptWriter
    {
        internal static bool TryReplaceClass(Type classType, GeneratableClass generatableClass)
        {
            return AssetsScriptGetter.TryGetSystemFilePathToScriptInAssets(classType, out UnityAssetPath unityAssetPath) &&
                   TryWrite(unityAssetPath, generatableClass.GenerateStringRepresentation());
        }

        internal static bool TryWrite(UnityAssetPath unityAssetPath, string contents) => Write(unityAssetPath, contents) != ScriptWriteResult.Failed;

        /// <summary>
        /// Write the file only if its contents differ from what is already there. Rewriting an identical
        /// file makes Unity reimport it and reload the domain, which is slow and happens on every
        /// generation run. Nothing is logged on success: a generator producing many files should report
        /// its own summary rather than one console entry per file.
        /// </summary>
        internal static ScriptWriteResult Write(UnityAssetPath unityAssetPath, string contents)
        {
            if (!unityAssetPath.IsValid)
            {
                return ScriptWriteResult.Failed;
            }

            string systemPath = unityAssetPath.SystemPath;

            try
            {
                if (File.Exists(systemPath) && File.ReadAllText(systemPath) == contents)
                {
                    return ScriptWriteResult.Unchanged;
                }

                string directoryPath = Path.GetDirectoryName(systemPath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(systemPath, contents);
                return ScriptWriteResult.Written;
            }
            catch (Exception e)
            {
                Debug.LogError($"File could not be written: {e.Message}");
                return ScriptWriteResult.Failed;
            }
        }
    }
}