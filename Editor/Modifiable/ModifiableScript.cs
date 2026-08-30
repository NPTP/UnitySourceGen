using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPTP.UnitySourceGen.Editor.Extensions.Internal;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Generatable.Directives;
using NPTP.UnitySourceGen.Editor.ScriptWriting;
using UnityEditor;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.Modifiable
{
    /// <summary>
    /// An existing script, opened for editing in place rather than regenerated from scratch. Use this when
    /// only part of a file is generated and the rest is hand-written.
    /// <code>
    /// SourceGen.GetScriptToModify&lt;MyClass&gt;()
    ///     .WithDirective("UnityEngine")
    ///     .WithCodeChunkInRegion("Generated", chunk, replaceExistingCodeInRegion: true)
    ///     .ExecuteModification(refreshAssets: true);
    /// </code>
    /// Nothing is written to disk until ExecuteModification is called.
    /// </summary>
    public class ModifiableScript
    {
        private readonly List<string> scriptLines;
        private readonly UnityAssetPath unityAssetPath;

        internal ModifiableScript(List<string> scriptLines, UnityAssetPath unityAssetPath)
        {
            this.scriptLines = scriptLines;
            this.unityAssetPath = unityAssetPath;
        }

        public ModifiableScript RemoveLinesContaining(string content)
        {
            RemoveLines(content);
            return this;
        }

        /// <summary>
        /// Write like WithDirective("UnityEngine"), rather than WithDirective("using UnityEditor; using UnityEngine;").
        /// </summary>
        public ModifiableScript WithDirective(string directive)
        {
            if (!string.IsNullOrEmpty(directive)) AddDirective(directive);
            return this;
        }

        public ModifiableScript WithAlias(string alias, Type originalType)
        {
            if (!string.IsNullOrEmpty(alias)) AddAlias(alias, originalType);
            return this;
        }

        public ModifiableScript WithComment(string comment)
        {
            if (!string.IsNullOrEmpty(comment)) AddComment(comment);
            return this;
        }

        /// <summary>
        /// Replace or extend the contents of a "#region name" block with the given chunk. The chunk is
        /// indented to match the code already in the region.
        /// </summary>
        public ModifiableScript WithCodeChunkInRegion(string regionName, GeneratableCodeChunk codeChunk, bool replaceExistingCodeInRegion)
        {
            PutCodeChunkInRegion(regionName, codeChunk, replaceExistingCodeInRegion);
            return this;
        }

        public void ExecuteModification(bool refreshAssets)
        {
            ExecuteModification();
            if (refreshAssets) AssetDatabase.Refresh();
        }

        internal void ExecuteModification()
        {
            try
            {
                File.WriteAllLines(unityAssetPath.SystemPath, scriptLines);
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not execute script modification: {e.Message}");
            }
        }

        internal void AddDirective(string directive)
        {
            Directive d = new Directive(directive);
            if (scriptLines.Any(line => d.Matches(line)))
                return;

            scriptLines.Insert(0, d);
        }

        internal void AddAlias(string alias, Type originalType)
        {
            Alias a = new Alias(alias, originalType);
            if (scriptLines.Any(line => a.Matches(line)))
                return;

            scriptLines.Insert(0, a);
        }

        internal void AddComment(string comment)
        {
            scriptLines.Add(new GeneratableComment(comment));
        }

        private void RemoveLines(string content)
        {
            for (int i = 0; i < scriptLines.Count;)
            {
                if (scriptLines[i].Contains(content))
                    scriptLines.RemoveAt(i);
                else
                    i++;
            }
        }

        internal void PutCodeChunkInRegion(string regionName, GeneratableCodeChunk codeChunk, bool replaceExistingCodeInRegion)
        {
            int regionStartLineIndex = -1;
            int regionEndLineIndex = -1;

            for (int i = 0; i < scriptLines.Count; i++)
            {
                string scriptLine = scriptLines[i];
                if (scriptLine.ContainsAll("#region", regionName))
                    regionStartLineIndex = i;
                else if (scriptLine.Contains("#endregion"))
                    regionEndLineIndex = i;

                if (regionStartLineIndex >= 0 && regionEndLineIndex > 0)
                    break;
            }

            if (regionStartLineIndex == -1 || regionEndLineIndex == -1)
            {
                return;
            }

            int regionInteriorStartIndex = regionStartLineIndex + 1;

            if (replaceExistingCodeInRegion && regionEndLineIndex > regionStartLineIndex + 1)
            {
                scriptLines.RemoveRange(regionInteriorStartIndex, regionEndLineIndex - regionInteriorStartIndex);
            }

            codeChunk.Indent = scriptLines[regionInteriorStartIndex].GetIndentLevel();
            scriptLines.InsertRange(regionInteriorStartIndex, codeChunk.GenerateStringRepresentationLines());
        }
    }
}
