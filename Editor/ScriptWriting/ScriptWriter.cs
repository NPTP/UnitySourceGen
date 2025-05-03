using System;
using System.IO;
using NPTP.UnitySourceGen.Editor.Generatable;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.UnitySourceGen.Editor.ScriptWriting
{
    internal static class ScriptWriter
    {
        internal static bool TryReplaceClass(Type classType, GeneratableClass generatableClass)
        {
            string scriptPath = AssetsScriptGetter.GetSystemFilePathToScriptInAssets(classType);
            return TryWrite(scriptPath, generatableClass.GenerateStringRepresentation());
        }
        
        private static bool TryWrite(string filePath, string contents)
        {
            try
            {
                int sepIndex = filePath.LastIndexOf(Path.DirectorySeparatorChar);
                if (sepIndex >= 0)
                {
                    string directoryPath = filePath.Remove(sepIndex);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                }

                File.WriteAllText(filePath, contents);

                Debug.Log($"{filePath} written successfully!");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"File could not be written: {e.Message}");
                return false;
            }
        }
    }
}
