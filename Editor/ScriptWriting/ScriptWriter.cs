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
        
        public static bool TryWrite(string filePath, string contents)
        {
            string fullPath = Path.GetFullPath(filePath);
            
            try
            {
                int sepIndex = fullPath.LastIndexOf(Path.DirectorySeparatorChar);
                if (sepIndex >= 0)
                {
                    string directoryPath = fullPath.Remove(sepIndex);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                }

                File.WriteAllText(fullPath, contents);

                Debug.Log($"{fullPath} written successfully!");
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
