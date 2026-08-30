using System;
using System.Collections.Generic;
using System.IO;
using NPTP.UnitySourceGen.Editor.Extensions;
using NPTP.UnitySourceGen.Editor.Extensions.Internal;
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
                                codeChunk.Indent = line.GetIndentLevel();
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