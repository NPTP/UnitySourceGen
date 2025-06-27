using System;
using System.Collections.Generic;
using System.IO;
using NPTP.UnitySourceGen.Editor.Extensions;
using NPTP.UnitySourceGen.Editor.Generatable;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.ScriptWriting
{
    internal static class ScriptWriter
    {
        private enum ReplaceState
        {
            WaitingForStartMarker,
            WaitingForEndMarker
        }
        
        internal static bool TryReplaceClass(Type classType, GeneratableClass generatableClass)
        {
            return AssetsScriptGetter.TryGetSystemFilePathToScriptInAssets(classType, out UnityAssetPath unityAssetPath) &&
                   TryWrite(unityAssetPath, generatableClass.GenerateStringRepresentation());
        }

        internal static bool TryReplaceSection(UnityAssetPath unityAssetPath, string[] sectionStartMarkers, string sectionEndMarker, GeneratableCodeChunk codeChunk)
        {
            List<string> lines = new();

            try
            {
                using StreamReader sr = new(unityAssetPath.SystemPath);
                ReplaceState replaceState = ReplaceState.WaitingForStartMarker;
                while (sr.ReadLine() is { } line)
                {
                    switch (replaceState)
                    {
                        case ReplaceState.WaitingForStartMarker:
                            lines.Add(line);
                            if (line.ContainsAll(sectionStartMarkers))
                            {
                                codeChunk.Indent = GetIndentLevel(line);
                                lines.AddRange(codeChunk.GenerateStringRepresentationLines());
                                replaceState = ReplaceState.WaitingForEndMarker;
                            }
                            break;
                        case ReplaceState.WaitingForEndMarker:
                            if (line.Contains(sectionEndMarker))
                            {
                                lines.Add(line);
                                replaceState = ReplaceState.WaitingForStartMarker;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"The file could not be read: {e.Message}");
                return false;
            }
            
            return TryWrite(unityAssetPath, lines);
        }

        private static bool TryWrite(UnityAssetPath unityAssetPath, IEnumerable<string> contentsLines) => TryWrite(unityAssetPath, contentsLines.LinesToString());
        internal static bool TryWrite(UnityAssetPath unityAssetPath, string contents)
        {
            string systemPath = unityAssetPath.SystemPath;

            try
            {
                int sepIndex = systemPath.LastIndexOf(Path.DirectorySeparatorChar);
                if (sepIndex >= 0)
                {
                    string directoryPath = systemPath.Remove(sepIndex);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                }

                File.WriteAllText(systemPath, contents);

                Debug.Log($"{systemPath} written successfully!");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"File could not be written: {e.Message}");
                return false;
            }
        }
        
        private static int GetIndentLevel(string line)
        {
            if (string.IsNullOrEmpty(line))
                return 0;

            int whitespaceLength = line.Length - line.TrimStart().Length;
            if (whitespaceLength == 0)
                return 0;

            string whitespace = line.Substring(0, whitespaceLength);
            int tabs = 0;
            int spaces = 0;
            for (int i = 0; i < whitespace.Length; i++)
            {
                switch (whitespace[i])
                {
                    case ' ':
                        spaces++;
                        break;
                    case '\t':
                        tabs++;
                        break;
                }
            }

            return tabs + (int)Math.Ceiling((decimal)(spaces / 4));
        }
    }
}